namespace backend.Application.Commands.Tasks;

/// <summary>
/// Command for updating a task
/// </summary>
public class UpdateTaskCommand
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

