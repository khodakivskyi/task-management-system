namespace backend.EFRepositories.DTOs;

/// <summary>
/// DTO for Task projection - contains only necessary fields
/// Demonstrates projection benefits: less data, automatic AsNoTracking
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int? Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public string OwnerName { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public string? CategoryName { get; set; }
    public string? ProjectName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
}

