using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class ProjectMemberServiceTests
{
    private readonly Mock<IProjectMemberRepository> _mockMemberRepository;
    private readonly Mock<IRepository<Project>> _mockProjectRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRepository<ProjectRole>> _mockRoleRepository;
    private readonly ProjectMemberService _service;

    public ProjectMemberServiceTests()
    {
        _mockMemberRepository = new Mock<IProjectMemberRepository>();
        _mockProjectRepository = new Mock<IRepository<Project>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRepository<ProjectRole>>();
        _service = new ProjectMemberService(
            _mockMemberRepository.Object,
            _mockProjectRepository.Object,
            _mockUserRepository.Object,
            _mockRoleRepository.Object);
    }

    [Fact]
    public async Task AddMemberAsync_WithValidData_ReturnsMember()
    {
        // Arrange
        var projectId = 1;
        var userId = 2;
        var roleId = 1;

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockRoleRepository.Setup(x => x.GetByIdAsync(roleId))
            .ReturnsAsync(new ProjectRole { Id = roleId, Name = "Developer" });
        _mockMemberRepository.Setup(x => x.GetByProjectAndUserAsync(projectId, userId))
            .ReturnsAsync((ProjectMember?)null);
        _mockMemberRepository.Setup(x => x.CreateAsync(It.IsAny<ProjectMember>()))
            .ReturnsAsync(1);

        // Act - owner (userId=1) adding member (userId=2)
        var result = await _service.AddMemberAsync(projectId, userId, roleId, 1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.ProjectId.Should().Be(projectId);
        result.UserId.Should().Be(userId);
        result.RoleId.Should().Be(roleId);
        result.JoinedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockMemberRepository.Verify(x => x.CreateAsync(It.IsAny<ProjectMember>()), Times.Once);
    }

    [Fact]
    public async Task AddMemberAsync_WithInvalidProjectId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.AddMemberAsync(0, 1, 1, 1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Project id must be greater than 0");
    }

    [Fact]
    public async Task AddMemberAsync_WithInvalidUserId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.AddMemberAsync(1, 0, 1, 1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("User id must be greater than 0");
    }

    [Fact]
    public async Task AddMemberAsync_WithInvalidRoleId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.AddMemberAsync(1, 1, 0, 1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Role id must be greater than 0");
    }

    [Fact]
    public async Task AddMemberAsync_WithNonExistentProject_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _service.AddMemberAsync(999, 1, 1, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project not found");
    }

    [Fact]
    public async Task AddMemberAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _service.AddMemberAsync(1, 999, 1, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task AddMemberAsync_WithExistingMember_ThrowsConflictException()
    {
        // Arrange
        var projectId = 1;
        var userId = 2;
        var roleId = 1;

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockRoleRepository.Setup(x => x.GetByIdAsync(roleId))
            .ReturnsAsync(new ProjectRole { Id = roleId, Name = "Developer" });
        _mockMemberRepository.Setup(x => x.GetByProjectAndUserAsync(projectId, userId))
            .ReturnsAsync(new ProjectMember { Id = 1, ProjectId = projectId, UserId = userId, RoleId = roleId });

        // Act
        var act = async () => await _service.AddMemberAsync(projectId, userId, roleId, 1);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage($"User {userId} is already a member of project {projectId}");
    }

    [Fact]
    public async Task AddMemberAsync_WithNonOwnerRequesting_ThrowsUnauthorizedException()
    {
        // Arrange - user 2 trying to add member to project owned by user 1
        var projectId = 1;
        var userId = 3;
        var roleId = 1;

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });

        // Act
        var act = async () => await _service.AddMemberAsync(projectId, userId, roleId, 2);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Only the project members owner can add this project members");
    }

    [Fact]
    public async Task RemoveMemberAsync_WithValidData_RemovesMember()
    {
        // Arrange
        var projectId = 1;
        var userId = 2;
        var member = new ProjectMember { Id = 1, ProjectId = projectId, UserId = userId, RoleId = 1 };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockMemberRepository.Setup(x => x.GetByProjectAndUserAsync(projectId, userId))
            .ReturnsAsync(member);
        _mockMemberRepository.Setup(x => x.DeleteAsync(member.Id))
            .ReturnsAsync(true);

        // Act
        await _service.RemoveMemberAsync(projectId, userId, 1);

        // Assert
        _mockMemberRepository.Verify(x => x.DeleteAsync(member.Id), Times.Once);
    }

    [Fact]
    public async Task RemoveMemberAsync_WithNonExistentMember_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockMemberRepository.Setup(x => x.GetByProjectAndUserAsync(1, 2))
            .ReturnsAsync((ProjectMember?)null);

        // Act
        var act = async () => await _service.RemoveMemberAsync(1, 2, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User is not a member of this project");
    }

    [Fact]
    public async Task RemoveMemberAsync_WhenRepositoryFails_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = 1;
        var userId = 2;
        var member = new ProjectMember { Id = 1, ProjectId = projectId, UserId = userId, RoleId = 1 };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockMemberRepository.Setup(x => x.GetByProjectAndUserAsync(projectId, userId))
            .ReturnsAsync(member);
        _mockMemberRepository.Setup(x => x.DeleteAsync(member.Id))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _service.RemoveMemberAsync(projectId, userId, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Failed to remove user from project");
    }

    [Fact]
    public async Task RemoveMemberAsync_WithNonOwnerRequesting_ThrowsUnauthorizedException()
    {
        // Arrange - user 2 trying to remove member from project owned by user 1
        var projectId = 1;
        var userId = 3;

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });

        // Act
        var act = async () => await _service.RemoveMemberAsync(projectId, userId, 2);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Only the project members owner can remove this project members");
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithValidData_UpdatesRole()
    {
        // Arrange
        var projectId = 1;
        var userId = 2;
        var newRoleId = 2;
        var member = new ProjectMember { Id = 1, ProjectId = projectId, UserId = userId, RoleId = 1 };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockMemberRepository.Setup(x => x.GetByProjectAndUserAsync(projectId, userId))
            .ReturnsAsync(member);
        _mockRoleRepository.Setup(x => x.GetByIdAsync(newRoleId))
            .ReturnsAsync(new ProjectRole { Id = newRoleId, Name = "Manager" });
        _mockMemberRepository.Setup(x => x.UpdateAsync(It.IsAny<ProjectMember>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateMemberRoleAsync(projectId, userId, newRoleId, 1);

        // Assert
        result.Should().NotBeNull();
        result.RoleId.Should().Be(newRoleId);
        _mockMemberRepository.Verify(x => x.UpdateAsync(It.IsAny<ProjectMember>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithNonExistentMember_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockMemberRepository.Setup(x => x.GetByProjectAndUserAsync(1, 2))
            .ReturnsAsync((ProjectMember?)null);

        // Act
        var act = async () => await _service.UpdateMemberRoleAsync(1, 2, 2, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User is not a member of this project");
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithNonExistentRole_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = 1;
        var userId = 2;
        var newRoleId = 999;
        var member = new ProjectMember { Id = 1, ProjectId = projectId, UserId = userId, RoleId = 1 };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockMemberRepository.Setup(x => x.GetByProjectAndUserAsync(projectId, userId))
            .ReturnsAsync(member);
        _mockRoleRepository.Setup(x => x.GetByIdAsync(newRoleId))
            .ReturnsAsync((ProjectRole?)null);

        // Act
        var act = async () => await _service.UpdateMemberRoleAsync(projectId, userId, newRoleId, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project role not found");
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithNonOwnerRequesting_ThrowsUnauthorizedException()
    {
        // Arrange - user 2 trying to update member role in project owned by user 1
        var projectId = 1;
        var userId = 3;
        var newRoleId = 2;

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });

        // Act
        var act = async () => await _service.UpdateMemberRoleAsync(projectId, userId, newRoleId, 2);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Only the project members owner can update role for this project members");
    }

    [Fact]
    public async Task GetProjectMembersAsync_WithValidProjectId_ReturnsMembers()
    {
        // Arrange
        var projectId = 1;
        var members = new List<ProjectMember>
        {
            new() { Id = 1, ProjectId = projectId, UserId = 1, RoleId = 1 },
            new() { Id = 2, ProjectId = projectId, UserId = 2, RoleId = 2 },
            new() { Id = 3, ProjectId = projectId, UserId = 3, RoleId = 1 }
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockMemberRepository.Setup(x => x.GetByProjectIdAsync(projectId))
            .ReturnsAsync(members);

        // Act
        var result = await _service.GetProjectMembersAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(m => m.ProjectId.Should().Be(projectId));
    }

    [Fact]
    public async Task GetProjectMembersAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetProjectMembersAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Project id must be greater than 0");
    }

    [Fact]
    public async Task GetProjectMembersAsync_WithNonExistentProject_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _service.GetProjectMembersAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project not found");
    }

    [Fact]
    public async Task GetProjectMembersAsync_WithNoMembers_ReturnsEmptyList()
    {
        // Arrange
        var projectId = 1;

        _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
            .ReturnsAsync(new Project { Id = projectId, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockMemberRepository.Setup(x => x.GetByProjectIdAsync(projectId))
            .ReturnsAsync(new List<ProjectMember>());

        // Act
        var result = await _service.GetProjectMembersAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
