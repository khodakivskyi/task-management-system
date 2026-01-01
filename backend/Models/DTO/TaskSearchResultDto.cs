namespace backend.Models.DTO;

/// <summary>
/// DTO for task search results (matches stored procedure output)
/// </summary>
public class TaskSearchResultDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int? Priority { get; set; }
    public DateTime? Deadline { get; set; }
}
