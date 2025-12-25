namespace backend.Models;

/// <summary>
/// DTO for Task with joined data from related tables
/// </summary>
public class TaskWithDetailsDto
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int StatusId { get; set; }
    public int? CategoryId { get; set; }
    public int? ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }

    // Status fields
    public string StatusName { get; set; } = string.Empty;
    public string? StatusColor { get; set; }

    // Category fields
    public string? CategoryName { get; set; }
    public string? CategoryColor { get; set; }

    // Owner fields
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerSurname { get; set; }
    public string OwnerLogin { get; set; } = string.Empty;

    public TaskWithDetailsDto() { }
}