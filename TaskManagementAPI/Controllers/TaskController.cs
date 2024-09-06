﻿using Microsoft.AspNetCore.Authorization;
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

            _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
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

    [HttpGet]
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

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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