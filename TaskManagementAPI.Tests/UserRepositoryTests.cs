using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Tests;

public class UserRepositoryTests
{
    private DbContextOptions<TaskManagementDbContext> CreateInMemoryDbContextOptions()
    {
        return new DbContextOptionsBuilder<TaskManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test
            .Options;
    }

    [Fact]
    public async Task CreateAsync_AddsUserToDatabase()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashed_password" };

            // Act
            await userRepository.CreateAsync(user);

            // Assert
            var retrievedUser = await context.Users.FindAsync(user.Id);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Username, retrievedUser.Username);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashed_password" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var retrievedUser = await userRepository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);

            // Act
            var retrievedUser = await userRepository.GetByIdAsync(Guid.NewGuid()); // Non-existent ID

            // Assert
            Assert.Null(retrievedUser);
        }
    }

    [Fact]
    public async Task GetByUsernameAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashed_password" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var retrievedUser = await userRepository.GetByUsernameAsync(user.Username);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Username, retrievedUser.Username);
        }
    }

    [Fact]
    public async Task GetByUsernameAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);

            // Act
            var retrievedUser = await userRepository.GetByUsernameAsync("nonexistentuser");

            // Assert
            Assert.Null(retrievedUser);
        }
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashed_password" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var retrievedUser = await userRepository.GetByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Email, retrievedUser.Email);
        }
    }

    [Fact]
    public async Task GetByEmailAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);

            // Act
            var retrievedUser = await userRepository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(retrievedUser);
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUserInDatabase()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashed_password" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            user.Username = "updateduser";
            await userRepository.UpdateAsync(user);

            // Assert
            var retrievedUser = await context.Users.FindAsync(user.Id);
            Assert.NotNull(retrievedUser);
            Assert.Equal("updateduser", retrievedUser.Username);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesUserFromDatabase()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var userRepository = new UserRepository(context);
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashed_password" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            await userRepository.DeleteAsync(user.Id);

            // Assert
            var retrievedUser = await context.Users.FindAsync(user.Id);
            Assert.Null(retrievedUser);
        }
    }
}