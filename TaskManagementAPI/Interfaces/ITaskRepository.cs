using TaskManagementAPI.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Interfaces;

public interface ITaskRepository
{
    Task<Models.Task> CreateAsync(Models.Task task);
    Task<Models.Task?> GetByIdAsync(Guid id);
    Task<List<Models.Task>> GetAllByUserIdAsync(
        Guid userId,
        Status? status = null,
        Priority? priority = null,
        string? sortBy = null,
        bool sortDescending = false
        );
    Task<Models.Task> UpdateAsync(Models.Task task);
    Task DeleteAsync(Guid id);
}