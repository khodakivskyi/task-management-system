namespace backend.Models.DTO;

/// <summary>
/// DTO for task search results (matches stored procedure output)
/// </summary>
public record TaskSearchResultDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string StatusName { get; init; } = string.Empty;
    public int? Priority { get; init; }
    public DateTime? Deadline { get; init; }
}