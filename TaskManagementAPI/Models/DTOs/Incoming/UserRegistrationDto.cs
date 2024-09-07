using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models.DTOs.Incoming;

public class UserRegistrationDto
{
    /// <summary>
    /// The desired username for the new user.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    /// <summary>
    /// The email address of the new user.
    /// </summary>
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// The password for the new user. Must be at least 8 characters long,
    /// and contain at least one uppercase letter, one lowercase letter,
    /// one number, and one special character.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
}