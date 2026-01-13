using backend.GraphQL.Mutations;
using backend.GraphQL.ProjectMembers.Inputs;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Mutations;

public class ProjectMembersMutationTests
{
    private readonly Mock<IProjectMemberService> _mockProjectMemberService;
    private readonly ProjectMembersMutation _mutation;

    public ProjectMembersMutationTests()
    {
        _mockProjectMemberService = new Mock<IProjectMemberService>();
        _mutation = new ProjectMembersMutation();
    }

    [Fact]
    public async Task AddProjectMember_WithValidInput_ReturnsMember()
    {
        // Arrange
        var input = new AddProjectMemberInput
        {
            ProjectId = 1,
            UserId = 2,
            RoleId = 1,
            RequestingUserId = 1
        };

        var member = new ProjectMember
        {
            Id = 1,
            ProjectId = input.ProjectId,
            UserId = input.UserId,
            RoleId = input.RoleId,
            JoinedAt = DateTime.UtcNow
        };

        _mockProjectMemberService.Setup(x => x.AddMemberAsync(input.ProjectId, input.UserId, input.RoleId, input.RequestingUserId))
            .ReturnsAsync(member);

        // Act
        var result = await _mutation.AddProjectMember(input, _mockProjectMemberService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.ProjectId.Should().Be(input.ProjectId);
        result.UserId.Should().Be(input.UserId);
        result.RoleId.Should().Be(input.RoleId);
        _mockProjectMemberService.Verify(x => x.AddMemberAsync(input.ProjectId, input.UserId, input.RoleId, input.RequestingUserId), Times.Once);
    }

    [Fact]
    public async Task RemoveProjectMember_WithValidInput_ReturnsTrue()
    {
        // Arrange
        var input = new RemoveProjectMemberInput
        {
            ProjectId = 1,
            UserId = 2,
            RequestingUserId = 1
        };

        _mockProjectMemberService.Setup(x => x.RemoveMemberAsync(input.ProjectId, input.UserId, input.RequestingUserId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mutation.RemoveProjectMember(input, _mockProjectMemberService.Object);

        // Assert
        result.Should().BeTrue();
        _mockProjectMemberService.Verify(x => x.RemoveMemberAsync(input.ProjectId, input.UserId, input.RequestingUserId), Times.Once);
    }

    [Fact]
    public async Task UpdateProjectMemberRole_WithValidInput_ReturnsMember()
    {
        // Arrange
        var input = new UpdateProjectMemberRoleInput
        {
            ProjectId = 1,
            UserId = 2,
            NewRoleId = 2,
            RequestingUserId = 1
        };

        var member = new ProjectMember
        {
            Id = 1,
            ProjectId = input.ProjectId,
            UserId = input.UserId,
            RoleId = input.NewRoleId,
            JoinedAt = DateTime.UtcNow.AddDays(-30)
        };

        _mockProjectMemberService.Setup(x => x.UpdateMemberRoleAsync(input.ProjectId, input.UserId, input.NewRoleId, input.RequestingUserId))
            .ReturnsAsync(member);

        // Act
        var result = await _mutation.UpdateProjectMemberRole(input, _mockProjectMemberService.Object);

        // Assert
        result.Should().NotBeNull();
        result.RoleId.Should().Be(input.NewRoleId);
        _mockProjectMemberService.Verify(x => x.UpdateMemberRoleAsync(input.ProjectId, input.UserId, input.NewRoleId, input.RequestingUserId), Times.Once);
    }
}
