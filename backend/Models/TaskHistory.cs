namespace backend.Models;

/// <summary>
/// Represents a history record of changes to a task
/// </summary>
public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }

    public TaskHistory() { }
}

