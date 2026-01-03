using backend.Models.DTO;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for Project Statistics
/// </summary>
public class ProjectStatisticsQuery
{
    public async Task<ProjectStatisticsDto?> GetProjectStatistics(
        int projectId,
        [Service] IProjectStatisticsService projectStatisticsService)
    {
        return await projectStatisticsService.GetStatisticsAsync(projectId);
    }

    public async Task<decimal> GetProjectProgress(
        int projectId,
        [Service] IProjectStatisticsService projectStatisticsService)
    {
        return await projectStatisticsService.GetProgressAsync(projectId);
    }

    public async Task<ProjectAnalyticsDto?> GetProjectAnalytics(
        int projectId,
        [Service] IProjectStatisticsService projectStatisticsService)
    {
        return await projectStatisticsService.GetAnalyticsAsync(projectId);
    }
}
