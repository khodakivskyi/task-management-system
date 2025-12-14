namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: Comment
/// Clean entity class without scaffolding annotations
/// Configuration is done through Fluent API in CommentConfiguration
/// </summary>
public class Comment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public User User { get; set; } = null!;
}

