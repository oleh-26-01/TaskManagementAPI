using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models.DTOs.Incoming;

public class UserRegistrationDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
}