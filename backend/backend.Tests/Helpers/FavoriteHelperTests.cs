using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class FavoriteHelperTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IEntityTypeRepository> _mockEntityTypeRepository;

    public FavoriteHelperTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEntityTypeRepository = new Mock<IEntityTypeRepository>();
    }

    [Fact]
    public async Task ValidateFavoriteAsync_WithValidData_DoesNotThrow()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new EntityType { Id = 1, Name = "task" });

        // Act
        var act = async () => await FavoriteHelper.ValidateFavoriteAsync(1, 1, _mockUserRepository.Object, _mockEntityTypeRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateFavoriteAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await FavoriteHelper.ValidateFavoriteAsync(999, 1, _mockUserRepository.Object, _mockEntityTypeRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task ValidateFavoriteAsync_WithNonExistentEntityType_ThrowsNotFoundException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((EntityType?)null);

        // Act
        var act = async () => await FavoriteHelper.ValidateFavoriteAsync(1, 999, _mockUserRepository.Object, _mockEntityTypeRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Entity type not found");
    }

    [Fact]
    public async Task ValidateFavoriteAsync_WithMultipleEntityTypes_DoesNotThrow()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new EntityType { Id = 2, Name = "project" });

        // Act
        var act = async () => await FavoriteHelper.ValidateFavoriteAsync(1, 2, _mockUserRepository.Object, _mockEntityTypeRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
