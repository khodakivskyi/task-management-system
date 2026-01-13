using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IRepository<Project>> _mockProjectRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ProjectService _projectService;

    public ProjectServiceTests()
    {
        _mockProjectRepository = new Mock<IRepository<Project>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _projectService = new ProjectService(_mockProjectRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidProject_ReturnsCreatedProject()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 1,
            Name = "Test Project",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockProjectRepository.Setup(x => x.CreateAsync(It.IsAny<Project>()))
            .ReturnsAsync(1);

        // Act
        var result = await _projectService.CreateAsync(project);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Project");
        result.Description.Should().Be("Test Description");
        _mockProjectRepository.Verify(x => x.CreateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidOwnerId_ThrowsNotFoundException()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 999,
            Name = "Test Project",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _projectService.CreateAsync(project);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task CreateAsync_WithNullName_ThrowsValidationException()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 1,
            Name = null!,
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });

        // Act
        var act = async () => await _projectService.CreateAsync(project);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ThrowsValidationException()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 1,
            Name = "",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });

        // Act
        var act = async () => await _projectService.CreateAsync(project);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProject()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            OwnerId = 1,
            Name = "Test Project",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(project);

        // Act
        var result = await _projectService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Project");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _projectService.GetByIdAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Project id must be greater than 0");
    }

    [Fact]
    public async Task GetByIdAsync_WithNegativeId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _projectService.GetByIdAsync(-1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _projectService.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = 1, Name = "Project 1", StartDate = DateTime.UtcNow },
            new() { Id = 2, OwnerId = 1, Name = "Project 2", StartDate = DateTime.UtcNow },
            new() { Id = 3, OwnerId = 2, Name = "Project 3", StartDate = DateTime.UtcNow }
        };

        _mockProjectRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(projects);

        // Act
        var result = await _projectService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Project 1");
    }

    [Fact]
    public async Task GetAllAsync_WithNoProjects_ReturnsEmptyList()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Project>());

        // Act
        var result = await _projectService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesProject()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 1,
            OwnerId = 1,
            Name = "Old Name",
            Description = "Old Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        var updatedProject = new Project
        {
            Id = 1,
            OwnerId = 1, // Same owner (authorized to update)
            Name = "New Name",
            Description = "New Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingProject);
        _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockProjectRepository.Setup(x => x.UpdateAsync(It.IsAny<Project>()))
            .ReturnsAsync(true);

        // Act
        var result = await _projectService.UpdateAsync(updatedProject);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New Description");
        result.OwnerId.Should().Be(1); // OwnerId should be preserved
        _mockProjectRepository.Verify(x => x.UpdateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var project = new Project
        {
            Id = 999,
            OwnerId = 1,
            Name = "Test Project",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _projectService.UpdateAsync(project);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project not found");
    }

    [Fact]
    public async Task UpdateAsync_CannotChangeOwnerId_PreservesOriginalOwner()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 1,
            OwnerId = 1,
            Name = "Test Project",
            Description = "Test",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        var updateData = new Project
        {
            Id = 1,
            OwnerId = 1,
            Name = "Updated Project",
            Description = "Updated",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingProject);
        _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockProjectRepository.Setup(x => x.UpdateAsync(It.IsAny<Project>()))
            .ReturnsAsync(true);

        // Act
        var result = await _projectService.UpdateAsync(updateData);

        // Assert
        result.OwnerId.Should().Be(1); // Original owner preserved
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Arrange
        var project = new Project
        {
            Id = 0,
            OwnerId = 1,
            Name = "Test",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var act = async () => await _projectService.UpdateAsync(project);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidIdAndOwner_DeletesProject()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            OwnerId = 1,
            Name = "Test Project",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(project);
        _mockProjectRepository.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        await _projectService.DeleteAsync(1, 1);

        // Assert
        _mockProjectRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _projectService.DeleteAsync(999, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Project not found");
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _projectService.DeleteAsync(0, 1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryFails_ThrowsNotFoundException()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            OwnerId = 1,
            Name = "Test Project",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(project);
        _mockProjectRepository.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _projectService.DeleteAsync(1, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Failed to delete project");
    }

    [Fact]
    public async Task DeleteAsync_WithDifferentOwner_ThrowsUnauthorizedException()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            OwnerId = 1,
            Name = "Test Project",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(project);

        // Act - user 2 trying to delete project owned by user 1
        var act = async () => await _projectService.DeleteAsync(1, 2);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Only the project owner can delete this project");
    }

    [Fact]
    public async Task UpdateAsync_WithDifferentOwner_ThrowsUnauthorizedException()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 1,
            OwnerId = 1,
            Name = "Test Project",
            Description = "Original Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        var updatedProject = new Project
        {
            Id = 1,
            OwnerId = 2, // Different owner trying to update
            Name = "Updated Project",
            Description = "Updated Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(60)
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingProject);

        // Act
        var act = async () => await _projectService.UpdateAsync(updatedProject);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Only the project owner can update this project");
    }
}
