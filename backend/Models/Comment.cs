namespace backend.Models;

/// <summary>
/// Represents a comment on a task
/// </summary>
public class Comment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Comment() { }
}

