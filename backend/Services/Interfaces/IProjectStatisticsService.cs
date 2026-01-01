using backend.Models.DTO;

namespace backend.Services.Interfaces;

public interface IProjectStatisticsService
{
    /// <summary>
    /// Project statistics including task counts and hours
    /// </summary>
    Task<ProjectStatisticsDto?> GetStatisticsAsync(int projectId);

    /// <summary>
    /// Progress percentage (0.00-100.00)
    /// </summary>
    Task<decimal> GetProgressAsync(int projectId);

    /// <summary>
    /// Gets detailed project analytics (statistics + progress combined)
    /// </summary>
    Task<ProjectAnalyticsDto?> GetAnalyticsAsync(int projectId);
}
