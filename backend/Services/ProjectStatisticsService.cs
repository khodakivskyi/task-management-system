using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTO;

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
        if (projectId <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        return await _projectStatisticRepository.GetAnalyticsAsync(projectId);
    }

    public async Task<decimal> GetProgressAsync(int projectId)
    {
        if (projectId <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        return await _projectStatisticRepository.GetProgressAsync(projectId);
    }

    public async Task<ProjectStatisticsDto?> GetStatisticsAsync(int projectId)
    {
        if (projectId <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        return await _projectStatisticRepository.GetStatisticsAsync(projectId);
    }
}
