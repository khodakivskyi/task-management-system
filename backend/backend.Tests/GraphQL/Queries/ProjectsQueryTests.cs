using backend.GraphQL.Queries;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Queries;

public class ProjectsQueryTests
{
    private readonly Mock<IProjectService> _mockProjectService;
    private readonly ProjectsQuery _query;

    public ProjectsQueryTests()
    {
        _mockProjectService = new Mock<IProjectService>();
        _query = new ProjectsQuery();
    }

    [Fact]
    public async Task GetProjects_ReturnsAllProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = 1, Name = "Project 1", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) },
            new() { Id = 2, Name = "Project 2", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(60) },
            new() { Id = 3, Name = "Project 3", OwnerId = 2, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(90) }
        };

        _mockProjectService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(projects);

        // Act
        var result = await _query.GetProjects(_mockProjectService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Project 1");
        result.Should().Contain(p => p.Name == "Project 2");
        _mockProjectService.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProjects_WithNoProjects_ReturnsEmptyList()
    {
        // Arrange
        _mockProjectService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Project>());

        // Act
        var result = await _query.GetProjects(_mockProjectService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProjectById_WithValidId_ReturnsProject()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Description = "Test Description",
            OwnerId = 1,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockProjectService.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(project);

        // Act
        var result = await _query.GetProjectById(1, _mockProjectService.Object);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Project");
        result.Description.Should().Be("Test Description");
        _mockProjectService.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetProjectById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockProjectService.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _query.GetProjectById(999, _mockProjectService.Object);

        // Assert
        result.Should().BeNull();
    }
}
