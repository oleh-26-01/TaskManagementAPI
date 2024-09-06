using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models.DTOs.Incoming;

namespace TaskManagementAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
    {
        try
        {
            var user = await _userService.RegisterAsync(registrationDto.Username, registrationDto.Email, registrationDto.Password);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}