using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Models.Task> CreateTaskAsync(Guid userId, string title, string? description, DateTime? dueDate, Status status, Priority priority)
    {
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

    public async Task<Models.Task?> GetTaskByIdAsync(Guid taskId)
    {
        return await _taskRepository.GetByIdAsync(taskId);
    }

    public async Task<List<Models.Task>> GetAllTasksByUserIdAsync(
        Guid userId,
        Status? status = null,
        Priority? priority = null,
        string? sortBy = null,
        bool sortDescending = false
    )
    {
        var tasks = await _taskRepository.GetAllByUserIdAsync(userId, status, priority, sortBy, sortDescending);
        return tasks;
    }

    public async Task<Models.Task> UpdateTaskAsync(Guid taskId, string? title, string? description, DateTime? dueDate, Status? status, Priority? priority)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null)
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

    public async Task DeleteTaskAsync(Guid taskId)
    {
        await _taskRepository.DeleteAsync(taskId);
    }
}