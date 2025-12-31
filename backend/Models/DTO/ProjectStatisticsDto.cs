namespace backend.Models.DTO;

/// <summary>
/// DTO for project statistics (matches get_project_statistics output)
/// </summary>
public class ProjectStatisticsDto
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TotalEstimatedHours { get; set; }
    public int TotalActualHours { get; set; }

    /// <summary>
    /// Calculated:  actual hours / estimated hours * 100
    /// </summary>
    public decimal? EfficiencyPercentage =>
        TotalEstimatedHours > 0
            ? Math.Round((decimal)TotalActualHours / TotalEstimatedHours * 100, 2)
            : null;

    /// <summary>
    /// Calculated: remaining hours = estimated - actual
    /// </summary>
    public int RemainingHours =>
        Math.Max(0, TotalEstimatedHours - TotalActualHours);
}
