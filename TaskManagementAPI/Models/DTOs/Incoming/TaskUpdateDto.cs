using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models.DTOs.Incoming;

public class TaskUpdateDto
{
    /// <summary>
    /// The updated title of the task.
    /// </summary>
    [StringLength(100)]
    public string? Title { get; set; }

    /// <summary>
    /// The updated description of the task.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// The updated due date for the task.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// The updated status of the task.
    /// </summary>
    public Status? Status { get; set; }

    /// <summary>
    /// The updated priority level of the task.
    /// </summary>
    public Priority? Priority { get; set; }
}