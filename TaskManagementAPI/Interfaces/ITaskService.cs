using TaskManagementAPI.Models;

using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Interfaces;

public interface ITaskService
{
    Task<Models.Task> CreateTaskAsync(Guid userId, string title, string? description, DateTime? dueDate, Status status, Priority priority);
    Task<Models.Task?> GetTaskByIdAsync(Guid taskId, Guid userId);
    Task<PagedList<Models.Task>> GetAllTasksByUserIdAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 10,
        Status? status = null,
        Priority? priority = null,
        DateTime? dueDate = null,
        string? sortBy = null,
        bool afterDueDate = false,
        bool sortDescending = false
    );
    Task<Models.Task> UpdateTaskAsync(Guid taskId, Guid userId, string? title, string? description, DateTime? dueDate, Status? status, Priority? priority);
    Task DeleteTaskAsync(Guid taskId, Guid userId);
}