﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        _passwordHasher = new PasswordHasher<User>();
    }

    private UserService CreateUserService()
    {
        return new UserService(
            Mock.Of<ILogger<UserService>>(),
            _mockUserRepository.Object,
            _mockConfiguration.Object,
            _passwordHasher
        );
    }

    [Fact]
    public async Task RegisterAsync_ValidInput_CreatesUser()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123!X";

        _mockUserRepository.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        var userService = CreateUserService();

        // Act
        var user = await userService.RegisterAsync(username, email, password);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.NotEmpty(user.PasswordHash);
        _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_UsernameExists_ThrowsException()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username))
            .ReturnsAsync(new User()); // Simulate an existing user with the same username

        var userService = CreateUserService();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => userService.RegisterAsync(username, email, password));
    }

    [Fact]
    public async Task RegisterAsync_EmailExists_ThrowsException()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";

        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email))
            .ReturnsAsync(new User()); // Simulate an existing user with the same email

        var userService = CreateUserService();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => userService.RegisterAsync(username, email, password));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var username = "testuser";
        var password = "password123!X";
        var user = new User { Username = username, PasswordHash = _passwordHasher.HashPassword(null, password) };

        var pseudoJwtSecret = "G/i7WhUprG35ymqYQpvDnUd6Y/jHGSg0i+c7DqN+b1M=";

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync(user);
        _mockConfiguration.SetupGet(config => config["Jwt:TestSecret"]).Returns(pseudoJwtSecret);

        var userService = CreateUserService();

        // Act
        var token = await userService.LoginAsync(username, password);

        // Assert
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsException()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";
        var user = new User { Username = username, PasswordHash = _passwordHasher.HashPassword(null, "password123") }; // Different password

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync(user);

        var userService = CreateUserService();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => userService.LoginAsync(username, password));
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "password123";

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync((User)null); // User not found

        var userService = CreateUserService();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => userService.LoginAsync(username, password));
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser", Email = "test@example.com", PasswordHash = "hashed_password" };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        var userService = CreateUserService();

        // Act
        var retrievedUser = await userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(userId, retrievedUser.Id);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null); // User not found

        var userService = CreateUserService();

        // Act
        var retrievedUser = await userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(retrievedUser);
    }

    [Theory]
    [InlineData("Password123!", true)] // Valid password
    [InlineData("password123", false)] // Missing uppercase and special character
    [InlineData("PASSWORD123!", false)] // Missing lowercase
    [InlineData("Password!", false)] // Missing digit
    [InlineData("P@wOrd1", false)] // Too short
    public void PasswordComplexityRegex_ValidatesPassword(string password, bool expectedIsValid)
    {
        // Act
        var isValid = UserService.PasswordComplexityRegex.IsMatch(password);

        // Assert
        Assert.Equal(expectedIsValid, isValid);
    }
}