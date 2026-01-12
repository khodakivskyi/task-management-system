namespace backend.Models.DTO;

/// <summary>
/// DTO for task search results (matches stored procedure output)
/// </summary>
public record TaskSearchResultDto(
    int Id,
    string Title,
    string StatusName,
    int? Priority,
    DateTime? Deadline
);
