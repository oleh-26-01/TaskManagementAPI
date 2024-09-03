using TaskManagementAPI.Models;

namespace TaskManagementAPI.Interfaces;

public interface IUserService
{
    Task<User> RegisterAsync(string username, string email, string password);
    Task<string> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(Guid id); // Add this method to get user by ID
}