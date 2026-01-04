namespace backend.Models.Filters;

/// <summary>
/// Filter object for task search
/// </summary>
public class TaskSearchFilter
{
    public int? UserId { get; set; }
    public int? ProjectId { get; set; }
    public int? StatusId { get; set; }
    public int? PriorityMin { get; set; }
    public int? PriorityMax { get; set; }
    public string? SearchText { get; set; }
}
