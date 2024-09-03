using TaskManagementAPI.Models;

using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email); // Add this method for email lookup
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}