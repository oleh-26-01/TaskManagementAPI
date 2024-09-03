using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> RegisterAsync(string username, string email, string password)
    {
        // Validate if username or email already exists
        if (await _userRepository.GetByUsernameAsync(username) != null)
        {
            throw new Exception("Username already exists.");
        }

        if (await _userRepository.GetByEmailAsync(email) != null)
        {
            throw new Exception("Email already exists.");
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
        return await _userRepository.CreateAsync(user);
    }

    public async Task<string> LoginAsync(string username, string password)
    {
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
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);
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
        return tokenHandler.WriteToken(token);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}