using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Tests;

public class TaskRepositoryTests
{
    private DbContextOptions<TaskManagementDbContext> CreateInMemoryDbContextOptions()
    {
        return new DbContextOptionsBuilder<TaskManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database name for each test
            .Options;
    }

    [Fact]
    public async Task CreateAsync_AddsTaskToDatabase()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var taskRepository = new TaskRepository(context);
            var task = new Models.Task
            {
                UserId = Guid.NewGuid(),
                Title = "Test Task",
                Description = "This is a test task.",
                DueDate = DateTime.Now.AddDays(7),
                Status = Status.Pending,
                Priority = Priority.High
            };

            // Act
            await taskRepository.CreateAsync(task);

            // Assert
            var retrievedTask = await context.Tasks.FindAsync(task.Id);
            Assert.NotNull(retrievedTask);
            Assert.Equal(task.Title, retrievedTask.Title);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ExistingTask_ReturnsTask()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var taskRepository = new TaskRepository(context);
            var task = new Models.Task { UserId = Guid.NewGuid(), Title = "Test Task" };
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();

            // Act
            var retrievedTask = await taskRepository.GetByIdAsync(task.Id);

            // Assert
            Assert.NotNull(retrievedTask);
            Assert.Equal(task.Id, retrievedTask.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingTask_ReturnsNull()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var taskRepository = new TaskRepository(context);

            // Act
            var retrievedTask = await taskRepository.GetByIdAsync(Guid.NewGuid()); // Non-existent ID

            // Assert
            Assert.Null(retrievedTask);
        }
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ReturnsTasksForUser()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();
        var userId = Guid.NewGuid();

        using (var context = new TaskManagementDbContext(options))
        {
            var taskRepository = new TaskRepository(context);
            await context.Tasks.AddRangeAsync(
                new Models.Task { UserId = userId, Title = "Task 1" },
                new Models.Task { UserId = userId, Title = "Task 2" },
                new Models.Task { UserId = Guid.NewGuid(), Title = "Task 3" } // Task for a different user
            );
            await context.SaveChangesAsync();

            // Act
            var retrievedTasks = await taskRepository.GetAllByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, retrievedTasks.Count);
            Assert.All(retrievedTasks, task => Assert.Equal(userId, task.UserId));
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTaskInDatabase()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var taskRepository = new TaskRepository(context);
            var task = new Models.Task { UserId = Guid.NewGuid(), Title = "Test Task", Status = Status.Pending };
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();

            // Act
            task.Title = "Updated Task";
            task.Status = Status.Completed;
            await taskRepository.UpdateAsync(task);

            // Assert
            var retrievedTask = await context.Tasks.FindAsync(task.Id);
            Assert.NotNull(retrievedTask);
            Assert.Equal("Updated Task", retrievedTask.Title);
            Assert.Equal(Status.Completed, retrievedTask.Status);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesTaskFromDatabase()
    {
        // Arrange
        var options = CreateInMemoryDbContextOptions();

        using (var context = new TaskManagementDbContext(options))
        {
            var taskRepository = new TaskRepository(context);
            var task = new Models.Task { UserId = Guid.NewGuid(), Title = "Test Task" };
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();

            // Act
            await taskRepository.DeleteAsync(task.Id);

            // Assert
            var retrievedTask = await context.Tasks.FindAsync(task.Id);
            Assert.Null(retrievedTask);
        }
    }
}