using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Interfaces;

public interface ITaskRepository
{
    Task<Models.Task> CreateAsync(Models.Task task);
    Task<Models.Task?> GetByIdAsync(Guid id);
    Task<List<Models.Task>> GetAllByUserIdAsync(Guid userId);
    Task<Models.Task> UpdateAsync(Models.Task task);
    Task DeleteAsync(Guid id);
    // TODO: Add methods for filtering and sorting tasks
}