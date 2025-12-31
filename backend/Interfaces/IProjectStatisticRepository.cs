using backend.Models.DTO;

namespace backend.Interfaces;

/// <summary>
/// Defines methods for Project statistics
/// </summary>
public interface IProjectStatisticRepository
{
    Task<ProjectStatisticsDto?> GetStatisticsAsync(int projectId);
    Task<decimal> GetProgressAsync(int projectId);
    Task<ProjectAnalyticsDto?> GetAnalyticsAsync(int projectId);
}
