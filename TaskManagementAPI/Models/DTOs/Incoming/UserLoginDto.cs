using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models.DTOs.Incoming;

public class UserLoginDto
{
    /// <summary>
    /// The username of the user.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    /// <summary>
    /// The password of the user.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
}