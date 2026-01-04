namespace backend.GraphQL.Tasks.Inputs;

/// <summary>
/// Input type for creating a new task
/// </summary>
public class CreateTaskInput
{
    public int OwnerId { get; set; }
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
