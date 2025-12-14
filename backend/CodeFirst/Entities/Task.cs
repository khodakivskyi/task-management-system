namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: Task
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in TaskConfiguration
/// </summary>
public class Task
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int StatusId { get; set; }
    public int? CategoryId { get; set; }
    public int? ProjectId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int? Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }

    // Navigation properties
    public User Owner { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public Category? Category { get; set; }
    public Project? Project { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TaskAssignee> TaskAssignees { get; set; } = new List<TaskAssignee>();
    public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
}

