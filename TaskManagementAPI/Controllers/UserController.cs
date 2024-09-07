using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models.DTOs.Incoming;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registrationDto">The user registration data.</param>
    /// <returns>An IActionResult containing the registered user or an error message.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">Bad request - Invalid input or user already exists.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Models.User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
    {
        try
        {
            var user = await _userService.RegisterAsync(registrationDto.Username, registrationDto.Email, registrationDto.Password);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user.");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">The user login data.</param>
    /// <returns>An IActionResult containing the JWT token or an error message.</returns>
    /// <response code="200">User logged in successfully.</response>
    /// <response code="400">Bad request - Invalid username or password.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var token = await _userService.LoginAsync(loginDto.Username, loginDto.Password);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user.");
            return BadRequest(new { message = ex.Message });
        }
    }
}