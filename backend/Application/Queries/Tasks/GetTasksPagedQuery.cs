namespace backend.Application.Queries.Tasks;

/// <summary>
/// Query for getting paginated tasks
/// </summary>
public class GetTasksPagedQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDirection { get; set; } = "DESC";
    public string? FilterValue { get; set; }
}

