using TaskManagementAPI.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Interfaces;

public interface ITaskRepository
{
    Task<Models.Task> CreateAsync(Models.Task task);
    Task<Models.Task?> GetByIdAsync(Guid id);
    Task<PagedList<Models.Task>> GetAllByUserIdAsync(
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
    Task<Models.Task> UpdateAsync(Models.Task task);
    Task DeleteAsync(Guid id);
}