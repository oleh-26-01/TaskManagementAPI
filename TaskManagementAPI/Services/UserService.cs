﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;

    public static readonly Regex PasswordComplexityRegex = new(
        "^"                     // Start of the string
        + "(?=.*[a-z])"         // At least one lowercase letter
        + "(?=.*[A-Z])"         // At least one uppercase letter
        + @"(?=.*\d)"           // At least one digit
        + @"(?=.*[^\da-zA-Z])"  // At least one special character (anything that is not a digit or letter)
        + ".{8,}$",             // At least 8 characters long
        RegexOptions.Compiled   // Compile the regex for better performance
    );

    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
    {
        _logger = logger;
        _userRepository = userRepository;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> RegisterAsync(string username, string email, string password)
    {
        _logger.LogInformation("Registering new user with username: {Username}", username);

        // Validate if username or email already exists
        if (await _userRepository.GetByUsernameAsync(username) != null)
        {
            throw new Exception("Username already exists.");
        }

        if (await _userRepository.GetByEmailAsync(email) != null)
        {
            throw new Exception("Email already exists.");
        }

        // Password complexity validation
        if (!PasswordComplexityRegex.IsMatch(password))
        {
            throw new Exception("Password must be at least 8 characters long " +
                                "and contain at least one lowercase letter, " +
                                "one uppercase letter, one digit, " +
                                "and one special character.");
        }

        // Hash the password
        var hashedPassword = _passwordHasher.HashPassword(null, password);

        // Create a new user object
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hashedPassword
        };

        // Save the user to the database
        var createdUser = await _userRepository.CreateAsync(user);

        _logger.LogInformation("User registered successfully with ID: {UserId}", createdUser.Id);

        return createdUser;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        _logger.LogInformation("User login attempt with username: {Username}", username);
        
        // Find the user by username
        var user = await _userRepository.GetByUsernameAsync(username);

        // If user not found, throw an exception
        if (user == null)
        {
            throw new Exception("Invalid username or password.");
        }

        // Verify the password
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        // If password is not valid, throw an exception
        if (result != PasswordVerificationResult.Success)
        {
            throw new Exception("Invalid username or password.");
        }

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:TestSecret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        _logger.LogInformation("User logged in successfully with ID: {UserId}", user.Id);

        return tokenHandler.WriteToken(token);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving user with ID: {UserId}", id);

        return await _userRepository.GetByIdAsync(id);
    }
}