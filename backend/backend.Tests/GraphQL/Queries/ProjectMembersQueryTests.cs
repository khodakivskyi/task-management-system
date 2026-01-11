using backend.GraphQL.Queries;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Queries;

public class ProjectMembersQueryTests
{
    private readonly Mock<IProjectMemberService> _mockProjectMemberService;
    private readonly ProjectMembersQuery _query;

    public ProjectMembersQueryTests()
    {
        _mockProjectMemberService = new Mock<IProjectMemberService>();
        _query = new ProjectMembersQuery();
    }

    [Fact]
    public async Task GetProjectMembers_WithValidProjectId_ReturnsMembers()
    {
        // Arrange
        var projectId = 1;
        var members = new List<ProjectMember>
        {
            new() { Id = 1, ProjectId = projectId, UserId = 1, RoleId = 1, JoinedAt = DateTime.UtcNow },
            new() { Id = 2, ProjectId = projectId, UserId = 2, RoleId = 2, JoinedAt = DateTime.UtcNow },
            new() { Id = 3, ProjectId = projectId, UserId = 3, RoleId = 1, JoinedAt = DateTime.UtcNow }
        };

        _mockProjectMemberService.Setup(x => x.GetProjectMembersAsync(projectId))
            .ReturnsAsync(members);

        // Act
        var result = await _query.GetProjectMembers(projectId, _mockProjectMemberService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(m => m.ProjectId.Should().Be(projectId));
        _mockProjectMemberService.Verify(x => x.GetProjectMembersAsync(projectId), Times.Once);
    }

    [Fact]
    public async Task GetProjectMembers_WithNoMembers_ReturnsEmptyList()
    {
        // Arrange
        var projectId = 1;
        _mockProjectMemberService.Setup(x => x.GetProjectMembersAsync(projectId))
            .ReturnsAsync(new List<ProjectMember>());

        // Act
        var result = await _query.GetProjectMembers(projectId, _mockProjectMemberService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
