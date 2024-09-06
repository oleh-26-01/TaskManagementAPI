using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Models;

public class Task
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public Status Status { get; set; } = Status.Pending;

    public Priority Priority { get; set; } = Priority.Medium;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Foreign key for the User (one-to-many relationship)
    public Guid UserId { get; set; }

    // Navigation property for the User
    [ForeignKey("UserId")]
    public User User { get; set; }
}

public enum Status
{
    Pending,
    InProgress,
    Completed
}

public enum Priority
{
    Low,
    Medium,
    High
}