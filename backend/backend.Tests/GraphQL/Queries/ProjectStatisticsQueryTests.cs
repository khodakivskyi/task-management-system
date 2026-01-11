using backend.GraphQL.Queries;
using backend.Models.DTO;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Queries;

public class ProjectStatisticsQueryTests
{
    private readonly Mock<IProjectStatisticsService> _mockStatisticsService;
    private readonly ProjectStatisticsQuery _query;

    public ProjectStatisticsQueryTests()
    {
        _mockStatisticsService = new Mock<IProjectStatisticsService>();
        _query = new ProjectStatisticsQuery();
    }

    [Fact]
    public async Task GetProjectAnalytics_WithValidId_ReturnsAnalytics()
    {
        // Arrange
        var projectId = 1;
        var analytics = new ProjectAnalyticsDto
        {
            ProjectId = projectId,
            ProgressPercentage = 50m,
            Statistics = new ProjectStatisticsDto
            {
                TotalTasks = 10,
                CompletedTasks = 5
            }
        };

        _mockStatisticsService.Setup(x => x.GetAnalyticsAsync(projectId))
            .ReturnsAsync(analytics);

        // Act
        var result = await _query.GetProjectAnalytics(projectId, _mockStatisticsService.Object);

        // Assert
        result.Should().NotBeNull();
        result!.ProjectId.Should().Be(projectId);
        _mockStatisticsService.Verify(x => x.GetAnalyticsAsync(projectId), Times.Once);
    }
}
