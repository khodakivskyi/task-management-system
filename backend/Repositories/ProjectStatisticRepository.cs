using backend.Interfaces;
using backend.Models.DTO;
using Dapper;

namespace backend.Repositories;

/// <summary>
/// Provides methods for retrieving analytics and statistical data related to projects.
/// </summary>
public class ProjectStatisticRepository : BaseRepository, IProjectStatisticRepository
{
    public ProjectStatisticRepository(string connectionString) : base(connectionString) { }

    public async Task<ProjectAnalyticsDto?> GetAnalyticsAsync(int projectId)
    {
        await using var connection = await GetConnectionAsync();

        var statisticsTask = GetStatisticsAsync(projectId);
        var progressTask = GetProgressAsync(projectId);

        await Task.WhenAll(statisticsTask, progressTask);

        var statistics = await statisticsTask;
        if (statistics == null)
        {
            return null;
        }

        return new ProjectAnalyticsDto
        {
            ProjectId = projectId,
            ProgressPercentage = await progressTask,
            Statistics = statistics
        };
    }

    public async Task<decimal> GetProgressAsync(int projectId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QuerySingleAsync<decimal>(
            "select calculate_project_progress(@project_id_param)",
            new { project_id_param = projectId }
        );
    }

    public async Task<ProjectStatisticsDto?> GetStatisticsAsync(int projectId)
    {
        await using var connection = await GetConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<ProjectStatisticsDto>(
           "select * from get_project_statistics(@project_id_param)",
           new { project_id_param = projectId }
       );
    }
}
