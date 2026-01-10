using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models.DTO;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class ProjectStatisticsServiceTests
{
    private readonly Mock<IProjectStatisticRepository> _mockRepository;
    private readonly ProjectStatisticsService _service;

    public ProjectStatisticsServiceTests()
    {
        _mockRepository = new Mock<IProjectStatisticRepository>();
        _service = new ProjectStatisticsService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAnalyticsAsync_WithValidProjectId_ReturnsAnalytics()
    {
        // Arrange
        var analytics = new ProjectAnalyticsDto
        {
            ProjectId = 1,
            ProgressPercentage = 50m,
            Statistics = new ProjectStatisticsDto
            {
                TotalTasks = 10,
                CompletedTasks = 5,
                InProgressTasks = 3,
                OverdueTasks = 2
            }
        };

        _mockRepository.Setup(x => x.GetAnalyticsAsync(1))
            .ReturnsAsync(analytics);

        // Act
        var result = await _service.GetAnalyticsAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.ProjectId.Should().Be(1);
        result.ProgressPercentage.Should().Be(50m);
        result.Statistics.TotalTasks.Should().Be(10);
        result.Statistics.CompletedTasks.Should().Be(5);
    }

    [Fact]
    public async Task GetAnalyticsAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetAnalyticsAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Project id must be greater than 0");
    }

    [Fact]
    public async Task GetAnalyticsAsync_WithNegativeId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetAnalyticsAsync(-1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetAnalyticsAsync_WithNonExistentProject_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAnalyticsAsync(999))
            .ReturnsAsync((ProjectAnalyticsDto?)null);

        // Act
        var result = await _service.GetAnalyticsAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetProgressAsync_WithValidProjectId_ReturnsProgress()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetProgressAsync(1))
            .ReturnsAsync(75.5m);

        // Act
        var result = await _service.GetProgressAsync(1);

        // Assert
        result.Should().Be(75.5m);
    }

    [Fact]
    public async Task GetProgressAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetProgressAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Project id must be greater than 0");
    }

    [Fact]
    public async Task GetProgressAsync_WithZeroTasks_ReturnsZero()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetProgressAsync(1))
            .ReturnsAsync(0m);

        // Act
        var result = await _service.GetProgressAsync(1);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public async Task GetProgressAsync_WithAllTasksCompleted_ReturnsHundred()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetProgressAsync(1))
            .ReturnsAsync(100m);

        // Act
        var result = await _service.GetProgressAsync(1);

        // Assert
        result.Should().Be(100m);
    }

    [Fact]
    public async Task GetStatisticsAsync_WithValidProjectId_ReturnsStatistics()
    {
        // Arrange
        var statistics = new ProjectStatisticsDto
        {
            TotalTasks = 15,
            CompletedTasks = 8,
            InProgressTasks = 5,
            OverdueTasks = 2,
            TotalEstimatedHours = 100,
            TotalActualHours = 60
        };

        _mockRepository.Setup(x => x.GetStatisticsAsync(1))
            .ReturnsAsync(statistics);

        // Act
        var result = await _service.GetStatisticsAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.TotalTasks.Should().Be(15);
        result.CompletedTasks.Should().Be(8);
        result.InProgressTasks.Should().Be(5);
        result.OverdueTasks.Should().Be(2);
        result.TotalEstimatedHours.Should().Be(100);
        result.TotalActualHours.Should().Be(60);
    }

    [Fact]
    public async Task GetStatisticsAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetStatisticsAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Project id must be greater than 0");
    }

    [Fact]
    public async Task GetStatisticsAsync_WithNonExistentProject_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetStatisticsAsync(999))
            .ReturnsAsync((ProjectStatisticsDto?)null);

        // Act
        var result = await _service.GetStatisticsAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
