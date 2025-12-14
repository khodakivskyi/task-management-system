namespace backend.CodeFirst.Entities;

/// <summary>
/// Code-First entity: TaskAttachment
/// Represents file attachments for tasks
/// Configuration is done through Fluent API in TaskAttachmentConfiguration
/// </summary>
public class TaskAttachment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UploadedById { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public long FileSize { get; set; } // Size in bytes
    public string ContentType { get; set; } = null!; // MIME type
    public DateTime UploadedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public User UploadedBy { get; set; } = null!;
}

