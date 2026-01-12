namespace backend.GraphQL.Tasks.Inputs;

/// <summary>
/// Input type for getting paginated tasks with filtering and sorting
/// </summary>
public class GetPagedTasksInput
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDirection { get; set; } = "DESC";
    public string? FilterValue { get; set; }
}
