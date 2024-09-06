using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Services;

public class TaskService : ITaskService
{
    private readonly ILogger<TaskService> _logger;
    private readonly ITaskRepository _taskRepository;

    public TaskService(ILogger<TaskService> logger, ITaskRepository taskRepository)
    {
        _logger = logger;
        _taskRepository = taskRepository;
    }

    public async Task<Models.Task> CreateTaskAsync(Guid userId, string title, string? description, DateTime? dueDate, Status status, Priority priority)
    {

        _logger.LogInformation("Creating a new task for user {UserId} with title: {Title}", userId, title); // Log task creation

        var task = new Models.Task
        {
            UserId = userId,
            Title = title,
            Description = description,
            DueDate = dueDate,
            Status = status,
            Priority = priority
        };

        return await _taskRepository.CreateAsync(task);
    }

    public async Task<Models.Task?> GetTaskByIdAsync(Guid taskId, Guid userId)
    {
        _logger.LogInformation("Retrieving task with ID: {TaskId}", taskId); // Log task retrieval

        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null || task.UserId != userId) // Check if task exists and belongs to the user
        {
            return null;
        }

        return task;
    }

    public async Task<PagedList<Models.Task>> GetAllTasksByUserIdAsync(
        Guid userId,
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
        var tasks = await _taskRepository
            .GetAllByUserIdAsync(userId, pageNumber, pageSize, status, priority, dueDate, 
                sortBy, afterDueDate, sortDescending);
        return tasks;
    }

    public async Task<Models.Task> UpdateTaskAsync(Guid taskId, Guid userId, string? title, string? description, DateTime? dueDate, Status? status, Priority? priority)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", taskId); // Log task update

        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null || task.UserId != userId) // Check if task exists and belongs to the user
        {
            throw new Exception("Task not found.");
        }

        task.Title = title ?? task.Title;
        task.Description = description ?? task.Description;
        task.DueDate = dueDate ?? task.DueDate;
        task.Status = status ?? task.Status;
        task.Priority = priority ?? task.Priority;

        return await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteTaskAsync(Guid taskId, Guid userId)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", taskId); // Log task deletion

        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null || task.UserId != userId) // Check if task exists and belongs to the user
        {
            throw new Exception("Task not found.");
        }

        await _taskRepository.DeleteAsync(taskId);
    }
}