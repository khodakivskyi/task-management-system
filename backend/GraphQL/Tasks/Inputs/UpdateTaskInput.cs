namespace backend.GraphQL.Tasks.Inputs;

/// <summary>
/// Input type for updating an existing task
/// </summary>
public class UpdateTaskInput
{
    public int Id { get; set; }
    public int StatusId { get; set; }
    public int? CategoryId { get; set; }
    public int? ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
}
