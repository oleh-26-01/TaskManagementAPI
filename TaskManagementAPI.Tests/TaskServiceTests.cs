using Moq;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;

    public TaskServiceTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
    }

    private TaskService CreateTaskService()
    {
        return new TaskService(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_ValidInput_CreatesTask()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = "Test Task";
        var description = "This is a test task.";
        var dueDate = DateTime.Now.AddDays(7);
        var status = Status.Pending;
        var priority = Priority.High;

        _mockTaskRepository.Setup(repo => repo.CreateAsync(It.IsAny<Models.Task>()))
            .ReturnsAsync((Models.Task task) => task);

        var taskService = CreateTaskService();

        // Act
        var createdTask = await taskService.CreateTaskAsync(userId, title, description, dueDate, status, priority);

        // Assert
        Assert.NotNull(createdTask);
        Assert.Equal(userId, createdTask.UserId);
        Assert.Equal(title, createdTask.Title);
        Assert.Equal(description, createdTask.Description);
        Assert.Equal(dueDate, createdTask.DueDate);
        Assert.Equal(status, createdTask.Status);
        Assert.Equal(priority, createdTask.Priority);
        _mockTaskRepository.Verify(repo => repo.CreateAsync(It.IsAny<Models.Task>()), Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ExistingTask_ReturnsTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new Models.Task { Id = taskId, Title = "Test Task" };

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

        var taskService = CreateTaskService();

        // Act
        var retrievedTask = await taskService.GetTaskByIdAsync(taskId);

        // Assert
        Assert.NotNull(retrievedTask);
        Assert.Equal(taskId, retrievedTask.Id);
    }

    [Fact]
    public async Task GetTaskByIdAsync_NonExistingTask_ReturnsNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync((Models.Task)null);

        var taskService = CreateTaskService();

        // Act
        var retrievedTask = await taskService.GetTaskByIdAsync(taskId);

        // Assert
        Assert.Null(retrievedTask);
    }

    [Fact]
    public async Task GetAllTasksByUserIdAsync_ReturnsTasks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tasks = new List<Models.Task>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, Title = "Task 1" },
            new() { Id = Guid.NewGuid(), UserId = userId, Title = "Task 2" }
        };

        _mockTaskRepository.Setup(repo => repo
            .GetAllByUserIdAsync(userId, null, null, null, null, false, false))
            .ReturnsAsync(tasks);

        var taskService = CreateTaskService();

        // Act
        var retrievedTasks = await taskService.GetAllTasksByUserIdAsync(userId);

        // Assert
        Assert.Equal(tasks.Count, retrievedTasks.Count);
        Assert.All(retrievedTasks, task => Assert.Equal(userId, task.UserId));
    }

    [Fact]
    public async Task UpdateTaskAsync_ExistingTask_UpdatesTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new Models.Task { Id = taskId, Title = "Test Task", Status = Status.Pending };
        var updatedTitle = "Updated Task";
        var updatedStatus = Status.Completed;

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Models.Task>()))
            .ReturnsAsync((Models.Task task) => task);

        var taskService = CreateTaskService();

        // Act
        var updatedTask = await taskService.UpdateTaskAsync(taskId, updatedTitle, null, null, updatedStatus, null);

        // Assert
        Assert.NotNull(updatedTask);
        Assert.Equal(updatedTitle, updatedTask.Title);
        Assert.Equal(updatedStatus, updatedTask.Status);
        _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Models.Task>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_NonExistingTask_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync((Models.Task)null);

        var taskService = CreateTaskService();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => taskService.UpdateTaskAsync(taskId, "Updated Task", null, null, Status.Completed, null));
    }

    [Fact]
    public async Task DeleteTaskAsync_DeletesTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mockTaskRepository.Setup(repo => repo.DeleteAsync(taskId));

        var taskService = CreateTaskService();

        // Act
        await taskService.DeleteTaskAsync(taskId);

        // Assert
        _mockTaskRepository.Verify(repo => repo.DeleteAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task GetAllTasksByUserIdAsync_WithFiltering_ReturnsFilteredTasks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tasks = new List<Models.Task>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, Title = "Task 1", Status = Status.Pending, Priority = Priority.High },
            new() { Id = Guid.NewGuid(), UserId = userId, Title = "Task 2", Status = Status.InProgress, Priority = Priority.Medium },
            new() { Id = Guid.NewGuid(), UserId = userId, Title = "Task 3", Status = Status.Completed, Priority = Priority.Low }
        };

        _mockTaskRepository.Setup(repo => repo
            .GetAllByUserIdAsync(userId, Status.InProgress, null, null, null, false, false))
            .ReturnsAsync(tasks.Where(t => t.Status == Status.InProgress).ToList());

        var taskService = CreateTaskService();

        // Act
        var retrievedTasks = await taskService.GetAllTasksByUserIdAsync(userId, status: Status.InProgress);

        // Assert
        Assert.Single(retrievedTasks);
        Assert.All(retrievedTasks, task => Assert.Equal(Status.InProgress, task.Status));
    }

    [Fact]
    public async Task GetAllTasksByUserIdAsync_WithSorting_ReturnsSortedTasks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tasks = new List<Models.Task>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, Title = "Task 1", DueDate = DateTime.Now.AddDays(3) },
            new() { Id = Guid.NewGuid(), UserId = userId, Title = "Task 2", DueDate = DateTime.Now.AddDays(1) },
            new() { Id = Guid.NewGuid(), UserId = userId, Title = "Task 3", DueDate = DateTime.Now.AddDays(2) }
        };

        _mockTaskRepository.Setup(repo => repo
            .GetAllByUserIdAsync(userId, null, null, null, "DueDate", false, false))
            .ReturnsAsync(tasks.OrderBy(t => t.DueDate).ToList());

        var taskService = CreateTaskService();

        // Act
        var retrievedTasks = await taskService.GetAllTasksByUserIdAsync(userId, sortBy: "DueDate");

        // Assert
        Assert.Equal(tasks.OrderBy(t => t.DueDate).Select(t => t.Id), retrievedTasks.Select(t => t.Id));
    }
}