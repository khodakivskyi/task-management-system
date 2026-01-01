using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models.DTO;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Provides methods for retrieving analytics, progress, and statistical information for projects.
/// </summary>
public class ProjectStatisticsService : IProjectStatisticsService
{
    private readonly IProjectStatisticRepository _projectStatisticRepository;
    public ProjectStatisticsService(IProjectStatisticRepository projectStatisticRepository)
    {
        _projectStatisticRepository = projectStatisticRepository ?? throw new ArgumentNullException(nameof(projectStatisticRepository));
    }

    public async Task<ProjectAnalyticsDto?> GetAnalyticsAsync(int projectId)
    {
        ValidationHelper.ValidateId(projectId, "Project");
        return await _projectStatisticRepository.GetAnalyticsAsync(projectId);
    }

    public async Task<decimal> GetProgressAsync(int projectId)
    {
        ValidationHelper.ValidateId(projectId, "Project");
        return await _projectStatisticRepository.GetProgressAsync(projectId);
    }

    public async Task<ProjectStatisticsDto?> GetStatisticsAsync(int projectId)
    {
        ValidationHelper.ValidateId(projectId, "Project");
        return await _projectStatisticRepository.GetStatisticsAsync(projectId);
    }
}
