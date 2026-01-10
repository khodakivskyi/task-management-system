using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class ProjectMemberHelperTests
{
    private readonly Mock<IRepository<Project>> _mockProjectRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRepository<ProjectRole>> _mockRoleRepository;

    public ProjectMemberHelperTests()
    {
        _mockProjectRepository = new Mock<IRepository<Project>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRepository<ProjectRole>>();
    }

    [Fact]
    public async Task ValidateProjectMemberAsync_WithValidData_DoesNotThrow()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new User { Id = 2, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockRoleRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new ProjectRole { Id = 1, Name = "Developer" });

        // Act
        var act = async () => await ProjectMemberHelper.ValidateProjectMemberAsync(
            1, 2, 1,
            _mockProjectRepository.Object,
            _mockUserRepository.Object,
            _mockRoleRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateProjectMemberAsync_WithNonExistentProject_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await ProjectMemberHelper.ValidateProjectMemberAsync(
            999, 1, 1,
            _mockProjectRepository.Object,
            _mockUserRepository.Object,
            _mockRoleRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project not found");
    }

    [Fact]
    public async Task ValidateProjectMemberAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await ProjectMemberHelper.ValidateProjectMemberAsync(
            1, 999, 1,
            _mockProjectRepository.Object,
            _mockUserRepository.Object,
            _mockRoleRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task ValidateProjectMemberAsync_WithNonExistentRole_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new User { Id = 2, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockRoleRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((ProjectRole?)null);

        // Act
        var act = async () => await ProjectMemberHelper.ValidateProjectMemberAsync(
            1, 2, 999,
            _mockProjectRepository.Object,
            _mockUserRepository.Object,
            _mockRoleRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project role not found");
    }

    [Fact]
    public async Task ValidateProjectMemberAsync_WithNullRoleRepository_DoesNotCheckRole()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, Name = "Test Project", OwnerId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) });
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new User { Id = 2, Login = "testuser", Email = "test@test.com", IsActive = true });

        // Act
        var act = async () => await ProjectMemberHelper.ValidateProjectMemberAsync(
            1, 2, 1,
            _mockProjectRepository.Object,
            _mockUserRepository.Object,
            null);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
