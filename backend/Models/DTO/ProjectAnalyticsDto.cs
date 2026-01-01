namespace backend.Models.DTO;

/// <summary>
/// Combined analytics DTO (statistics + progress)
/// </summary>
public class ProjectAnalyticsDto
{
    public int ProjectId { get; set; }
    public decimal ProgressPercentage { get; set; }
    public ProjectStatisticsDto Statistics { get; set; } = new();

    /// <summary>
    /// Project status based on progress
    /// </summary>
    public string Status => ProgressPercentage switch
    {
        0 => "not started",
        < 30 => "just started",
        < 70 => "in progress",
        < 100 => "almost done",
        100 => "completed",
        _ => "unknown"
    };

    /// <summary>
    /// Health indicator based on overdue tasks
    /// </summary>
    public string Health => Statistics.OverdueTasks switch
    {
        0 => "healthy",
        1 => "minor issues",
        <= 5 => "attention needed",
        _ => "critical"
    };
}
