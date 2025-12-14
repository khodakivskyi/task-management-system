namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: TaskAssignee
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in TaskAssigneeConfiguration
/// Represents many-to-many relationship between Task and User
/// </summary>
public class TaskAssignee
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public User User { get; set; } = null!;
}

