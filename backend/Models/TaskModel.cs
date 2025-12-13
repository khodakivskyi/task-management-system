namespace backend.Models;

/// <summary>
/// Represents a task in the system
/// </summary>
public class TaskModel
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int StatusId { get; set; }
    public int? CategoryId { get; set; }
    public int? ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Priority { get; set; } // Range: 1-5
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }

    public TaskModel() { }
}

