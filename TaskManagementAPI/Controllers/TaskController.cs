using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using TaskManagementAPI.Models.DTOs.Incoming;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize] // Require authorization for all actions in this controller
public class TaskController : ControllerBase
{
    private readonly ILogger<TaskService> _logger;
    private readonly ITaskService _taskService;

    public TaskController(ILogger<TaskService> logger, ITaskService taskService)
    {
        _logger = logger;
        _taskService = taskService;
    }

    /// <summary>
    /// Creates a new task for the current user.
    /// </summary>
    /// <param name="taskDto">The task data.</param>
    /// <returns>An IActionResult containing the created task or an error message.</returns>
    /// <response code="201">Task created successfully.</response>
    /// <response code="400">Bad request - Invalid input.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Models.Task), StatusCodes.Status201Created)] // Created
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto taskDto)
    {
        try
        {
            // Get the user ID from the JWT token
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var task = await _taskService.CreateTaskAsync(
                userId,
                taskDto.Title,
                taskDto.Description,
                taskDto.DueDate,
                taskDto.Status,
                taskDto.Priority
            );

            _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task.");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a task by ID for the current user.
    /// </summary>
    /// <param name="id">The ID of the task.</param>
    /// <returns>An IActionResult containing the task or an error message.</returns>
    /// <response code="200">Task retrieved successfully.</response>
    /// <response code="404">Not found - Task not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Models.Task), StatusCodes.Status200OK)] // OK
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        // Get the user ID from the JWT token
        var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var task = await _taskService.GetTaskByIdAsync(id, userId);

        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    /// <summary>
    /// Retrieves all tasks for the current user, with options for filtering, sorting, and pagination.
    /// </summary>
    /// <param name="pageNumber">The page number for pagination (default: 1).</param>
    /// <param name="pageSize">The number of tasks per page (default: 10).</param>
    /// <param name="status">Filter tasks by status (Pending, InProgress, Completed).</param>
    /// <param name="priority">Filter tasks by priority (Low, Medium, High).</param>
    /// <param name="dueDate">Filter tasks by due date. If 'afterDueDate' is true, filters tasks with due dates after the specified date. Otherwise, filters tasks with due dates on or before the specified date.</param>
    /// <param name="sortBy">Sort tasks by a specific field ('DueDate' or 'Priority').</param>
    /// <param name="afterDueDate">If true, filters tasks with due dates after the specified 'dueDate'. If false, filters tasks with due dates on or before the specified 'dueDate'.</param>
    /// <param name="sortDescending">If true, sorts tasks in descending order. If false, sorts tasks in ascending order.</param>
    /// <returns>An IActionResult containing a paged list of tasks or an error message.</returns>
    /// <response code="200">Tasks retrieved successfully.</response>
    /// <response code="400">Bad request - Invalid input.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<Models.Task>), StatusCodes.Status200OK)] // OK
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllTasksForCurrentUser(
        int pageNumber = 1,
        int pageSize = 10,
        Status? status = null,
        Priority? priority = null,
        DateTime? dueDate = null,
        string? sortBy = null,
        bool afterDueDate = false,
        bool sortDescending = false
    )
    {
        try
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tasks = await _taskService
                .GetAllTasksByUserIdAsync(userId, pageNumber, pageSize, status, priority, dueDate, 
                    sortBy, afterDueDate, sortDescending);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates a task by ID for the current user.
    /// </summary>
    /// <param name="id">The ID of the task.</param>
    /// <param name="taskDto">The updated task data.</param>
    /// <returns>An IActionResult containing the updated task or an error message.</returns>
    /// <response code="200">Task updated successfully.</response>
    /// <response code="400">Bad request - Invalid input or task not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Models.Task), StatusCodes.Status200OK)] // OK
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskUpdateDto taskDto)
    {
        try
        {
            // Get the user ID from the JWT token
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var task = await _taskService.UpdateTaskAsync(
                id,
                userId,
                taskDto.Title,
                taskDto.Description,
                taskDto.DueDate,
                taskDto.Status,
                taskDto.Priority
            );

            _logger.LogInformation("Task updated successfully with ID: {TaskId}", task.Id);

            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task with ID: {TaskId}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a task by ID for the current user.
    /// </summary>
    /// <param name="id">The ID of the task.</param>
    /// <returns>An IActionResult indicating success or an error message.</returns>
    /// <response code="204">Task deleted successfully.</response>
    /// <response code="400">Bad request - Task not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] // NoContent
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        try
        {
            // Get the user ID from the JWT token
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            await _taskService.DeleteTaskAsync(id, userId);

            _logger.LogInformation("Task deleted successfully with ID: {TaskId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task with ID: {TaskId}", id);
            return BadRequest(new { message = ex.Message });
        }
    }
}