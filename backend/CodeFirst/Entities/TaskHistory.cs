namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: TaskHistory
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in TaskHistoryConfiguration
/// </summary>
public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string FieldName { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public User User { get; set; } = null!;
}

