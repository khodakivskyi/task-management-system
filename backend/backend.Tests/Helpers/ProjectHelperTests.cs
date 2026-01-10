using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class ProjectHelperTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;

    public ProjectHelperTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
    }

    [Fact]
    public async Task ValidateProjectAsync_WithValidProject_DoesNotThrow()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 1,
            Name = "Test Project",
            Description = "Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });

        // Act
        var act = async () => await ProjectHelper.ValidateProjectAsync(project, _mockUserRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateProjectAsync_WithNullName_ThrowsValidationException()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 1,
            Name = null!,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var act = async () => await ProjectHelper.ValidateProjectAsync(project, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Project name is required");
    }

    [Fact]
    public async Task ValidateProjectAsync_WithEmptyName_ThrowsValidationException()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 1,
            Name = "",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var act = async () => await ProjectHelper.ValidateProjectAsync(project, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Project name is required");
    }

    [Fact]
    public async Task ValidateProjectAsync_WithWhitespaceName_ThrowsValidationException()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 1,
            Name = "   ",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var act = async () => await ProjectHelper.ValidateProjectAsync(project, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Project name is required");
    }

    [Fact]
    public async Task ValidateProjectAsync_WithEndDateBeforeStartDate_ThrowsValidationException()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 1,
            Name = "Test Project",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(-30)
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });

        // Act
        var act = async () => await ProjectHelper.ValidateProjectAsync(project, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("End date must be after start date");
    }

    [Fact]
    public async Task ValidateProjectAsync_WithNonExistentOwner_ThrowsNotFoundException()
    {
        // Arrange
        var project = new Project
        {
            OwnerId = 999,
            Name = "Test Project",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await ProjectHelper.ValidateProjectAsync(project, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task ValidateProjectAsync_WithSameStartAndEndDate_DoesNotThrow()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var project = new Project
        {
            OwnerId = 1,
            Name = "Test Project",
            StartDate = date,
            EndDate = date
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });

        // Act
        var act = async () => await ProjectHelper.ValidateProjectAsync(project, _mockUserRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
