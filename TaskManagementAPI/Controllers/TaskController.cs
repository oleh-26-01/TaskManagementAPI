using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using TaskManagementAPI.Models.DTOs.Incoming;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize] // Require authorization for all actions in this controller
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
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

            return CreatedAtAction(nameof(GetTaskById), new { taskId = task.Id }, task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskById(Guid taskId)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);

        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasksForCurrentUser(
        Status? status = null,
        Priority? priority = null,
        string? sortBy = null,
        bool sortDescending = false
    )
    {
        try
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tasks = await _taskService.GetAllTasksByUserIdAsync(userId, status, priority, sortBy, sortDescending);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskUpdateDto taskDto)
    {
        try
        {
            var task = await _taskService.UpdateTaskAsync(
                taskId,
                taskDto.Title,
                taskDto.Description,
                taskDto.DueDate,
                taskDto.Status,
                taskDto.Priority
            );

            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        try
        {
            await _taskService.DeleteTaskAsync(taskId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}