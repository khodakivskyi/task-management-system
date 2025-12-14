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
    public int? ProjectId { get; set; }
    public string Title { get; set; } = null!;
    public string? Details { get; set; } // Renamed from Description
    public int? Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    public string? Tags { get; set; } 
    public string? Status { get; set; } 

    // Computed property (configured in Fluent API)
    public decimal? ProgressPercentage { get; set; }

    // Navigation properties - only User, Task, Project relationships
    public User Owner { get; set; } = null!;
    public Project? Project { get; set; }
}

