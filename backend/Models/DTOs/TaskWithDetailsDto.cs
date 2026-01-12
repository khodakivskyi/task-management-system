namespace backend.Models.DTO;

/// <summary>
/// DTO for Task with joined data from related tables
/// </summary>
public record TaskWithDetailsDto(
    int Id,
    int OwnerId,
    int StatusId,
    int? CategoryId,
    int? ProjectId,
    string Title,
    string? Description,
    int? Priority,
    DateTime? Deadline,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int EstimatedHours,
    int ActualHours,
    string StatusName,
    string? StatusColor,
    string? CategoryName,
    string? CategoryColor,
    string OwnerName,
    string? OwnerSurname,
    string OwnerLogin
);
