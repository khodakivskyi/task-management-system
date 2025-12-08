namespace backend.Models;

/// <summary>
/// Represents a user assigned to a task
/// </summary>
public class TaskAssignee
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }

    public TaskAssignee() { }
}

