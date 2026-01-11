using backend.GraphQL.Mutations;
using backend.GraphQL.Projects.Inputs;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Mutations;

public class ProjectsMutationTests
{
    private readonly Mock<IProjectService> _mockProjectService;
    private readonly ProjectsMutation _mutation;

    public ProjectsMutationTests()
    {
        _mockProjectService = new Mock<IProjectService>();
        _mutation = new ProjectsMutation();
    }

    [Fact]
    public async Task CreateProject_WithValidInput_ReturnsCreatedProject()
    {
        // Arrange
        var input = new CreateProjectInput
        {
            OwnerId = 1,
            Name = "New Project",
            Description = "Project Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        var createdProject = new Project
        {
            Id = 1,
            OwnerId = input.OwnerId,
            Name = input.Name,
            Description = input.Description,
            StartDate = input.StartDate,
            EndDate = input.EndDate
        };

        _mockProjectService.Setup(x => x.CreateAsync(It.IsAny<Project>()))
            .ReturnsAsync(createdProject);

        // Act
        var result = await _mutation.CreateProject(input, _mockProjectService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("New Project");
        result.Description.Should().Be("Project Description");
        _mockProjectService.Verify(x => x.CreateAsync(It.Is<Project>(p =>
            p.OwnerId == input.OwnerId &&
            p.Name == input.Name &&
            p.Description == input.Description
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateProject_WithValidInput_ReturnsUpdatedProject()
    {
        // Arrange
        var input = new UpdateProjectInput
        {
            Id = 1,
            Name = "Updated Project",
            Description = "Updated Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(60)
        };

        var updatedProject = new Project
        {
            Id = input.Id,
            Name = input.Name,
            Description = input.Description,
            StartDate = input.StartDate,
            EndDate = input.EndDate,
            OwnerId = 1
        };

        _mockProjectService.Setup(x => x.UpdateAsync(It.IsAny<Project>()))
            .ReturnsAsync(updatedProject);

        // Act
        var result = await _mutation.UpdateProject(input, _mockProjectService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Updated Project");
        result.Description.Should().Be("Updated Description");
        _mockProjectService.Verify(x => x.UpdateAsync(It.Is<Project>(p =>
            p.Id == input.Id &&
            p.Name == input.Name &&
            p.Description == input.Description
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteProject_WithValidId_ReturnsTrue()
    {
        // Arrange
        var projectId = 1;
        _mockProjectService.Setup(x => x.DeleteAsync(projectId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mutation.DeleteProject(projectId, _mockProjectService.Object);

        // Assert
        result.Should().BeTrue();
        _mockProjectService.Verify(x => x.DeleteAsync(projectId), Times.Once);
    }
}
