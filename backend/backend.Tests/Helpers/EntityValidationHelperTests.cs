using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class EntityValidationHelperTests
{
    private readonly Mock<IRepository<User>> _mockRepository;

    public EntityValidationHelperTests()
    {
        _mockRepository = new Mock<IRepository<User>>();
    }

    [Fact]
    public async Task EnsureEntityExistsAsync_WithExistingEntity_DoesNotThrow()
    {
        // Arrange
        var user = new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true };
        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await backend.Helpers.EntityValidationHelper.EnsureEntityExistsAsync(1, _mockRepository.Object, "User");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Login.Should().Be("testuser");
    }

    [Fact]
    public async Task EnsureEntityExistsAsync_WithNonExistentEntity_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await backend.Helpers.EntityValidationHelper.EnsureEntityExistsAsync(999, _mockRepository.Object, "User");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task EnsureEntityExistsAsync_WithValidEntity_ReturnsEntity()
    {
        // Arrange
        var user = new User { Id = 5, Login = "user5", Email = "user5@test.com", IsActive = true };
        _mockRepository.Setup(x => x.GetByIdAsync(5))
            .ReturnsAsync(user);

        // Act
        var result = await backend.Helpers.EntityValidationHelper.EnsureEntityExistsAsync(5, _mockRepository.Object, "User");

        // Assert
        result.Should().BeSameAs(user);
    }

    [Fact]
    public async Task EnsureEntityExistsIfProvidedAsync_WithNull_DoesNotThrow()
    {
        // Act
        var act = async () => await backend.Helpers.EntityValidationHelper.EnsureEntityExistsIfProvidedAsync<User>(null, _mockRepository.Object, "User");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureEntityExistsIfProvidedAsync_WithValidId_DoesNotThrow()
    {
        // Arrange
        var user = new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true };
        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var act = async () => await backend.Helpers.EntityValidationHelper.EnsureEntityExistsIfProvidedAsync(1, _mockRepository.Object, "User");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureEntityExistsIfProvidedAsync_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await backend.Helpers.EntityValidationHelper.EnsureEntityExistsIfProvidedAsync(999, _mockRepository.Object, "User");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task EnsureEntityExistsIfProvidedAsync_WithNullRepository_DoesNotThrow()
    {
        // Act
        var act = async () => await backend.Helpers.EntityValidationHelper.EnsureEntityExistsIfProvidedAsync<User>(1, null, "User");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureEntityExistsIfProvidedAsync_WithNullIdAndNullRepository_DoesNotThrow()
    {
        // Act
        var act = async () => await backend.Helpers.EntityValidationHelper.EnsureEntityExistsIfProvidedAsync<User>(null, null, "User");

        // Assert
        await act.Should().NotThrowAsync();
    }
}
