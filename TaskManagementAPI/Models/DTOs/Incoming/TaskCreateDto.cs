using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Models;

public class TaskCreateDto
{
    /// <summary>
    /// The title of the task.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    /// <summary>
    /// A detailed description of the task.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// The due date for the task.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// The current status of the task.
    /// </summary>
    public Status Status { get; set; } = Status.Pending;

    /// <summary>
    /// The priority level of the task.
    /// </summary>
    public Priority Priority { get; set; } = Priority.Medium;
}