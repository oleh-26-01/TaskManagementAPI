using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models.DTOs.Incoming;

public class UserLoginDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
}