namespace backend.EFModels.ValueObjects;

/// <summary>
/// Task metadata value object
/// Used with JSON value converter to store as JSON string in database
/// </summary>
public class TaskMetadata
{
    public string? Tags { get; set; }
    public string? Notes { get; set; }
    public Dictionary<string, string>? CustomFields { get; set; }
    public DateTime? LastReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
}


