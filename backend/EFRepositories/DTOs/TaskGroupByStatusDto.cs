namespace backend.EFRepositories.DTOs;

/// <summary>
/// DTO for grouping tasks by status with aggregation
/// </summary>
public class TaskGroupByStatusDto
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;
    public int TaskCount { get; set; }
    public int? AveragePriority { get; set; }
    public int TotalEstimatedHours { get; set; }
    public int TotalActualHours { get; set; }
}


