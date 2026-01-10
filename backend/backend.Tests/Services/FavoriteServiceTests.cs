using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class FavoriteServiceTests
{
    private readonly Mock<IFavoriteRepository> _mockFavoriteRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IEntityTypeRepository> _mockEntityTypeRepository;
    private readonly FavoriteService _service;

    public FavoriteServiceTests()
    {
        _mockFavoriteRepository = new Mock<IFavoriteRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEntityTypeRepository = new Mock<IEntityTypeRepository>();
        _service = new FavoriteService(
            _mockFavoriteRepository.Object,
            _mockUserRepository.Object,
            _mockEntityTypeRepository.Object);
    }

    [Fact]
    public async Task AddAsync_WithValidData_ReturnsFavorite()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new EntityType { Id = 1, Name = "Task" });
        _mockFavoriteRepository.Setup(x => x.GetByUserAndEntityAsync(1, 1, 1))
            .ReturnsAsync((Favorite?)null);
        _mockFavoriteRepository.Setup(x => x.CreateAsync(It.IsAny<Favorite>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.AddAsync(1, 1, 1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.UserId.Should().Be(1);
        result.EntityTypeId.Should().Be(1);
        result.EntityId.Should().Be(1);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockFavoriteRepository.Verify(x => x.CreateAsync(It.IsAny<Favorite>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithInvalidUserId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.AddAsync(0, 1, 1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("User id must be greater than 0");
    }

    [Fact]
    public async Task AddAsync_WithInvalidEntityTypeId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.AddAsync(1, 0, 1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Entity type id must be greater than 0");
    }

    [Fact]
    public async Task AddAsync_WithInvalidEntityId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.AddAsync(1, 1, 0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Entity id must be greater than 0");
    }

    [Fact]
    public async Task AddAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _service.AddAsync(999, 1, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task AddAsync_WithNonExistentEntityType_ThrowsNotFoundException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((EntityType?)null);

        // Act
        var act = async () => await _service.AddAsync(1, 999, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Entity type not found");
    }

    [Fact]
    public async Task AddAsync_WithExistingFavorite_ThrowsConflictException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockEntityTypeRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new EntityType { Id = 1, Name = "Task" });
        _mockFavoriteRepository.Setup(x => x.GetByUserAndEntityAsync(1, 1, 1))
            .ReturnsAsync(new Favorite { Id = 1, UserId = 1, EntityTypeId = 1, EntityId = 1 });

        // Act
        var act = async () => await _service.AddAsync(1, 1, 1);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("This item is already in favorites");
    }

    [Fact]
    public async Task RemoveAsync_WithValidData_RemovesFavorite()
    {
        // Arrange
        var favorite = new Favorite
        {
            Id = 1,
            UserId = 1,
            EntityTypeId = 1,
            EntityId = 1,
            CreatedAt = DateTime.UtcNow
        };

        _mockFavoriteRepository.Setup(x => x.GetByUserAndEntityAsync(1, 1, 1))
            .ReturnsAsync(favorite);
        _mockFavoriteRepository.Setup(x => x.DeleteByUserAndEntityAsync(1, 1, 1))
            .ReturnsAsync(true);

        // Act
        await _service.RemoveAsync(1, 1, 1);

        // Assert
        _mockFavoriteRepository.Verify(x => x.DeleteByUserAndEntityAsync(1, 1, 1), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WithInvalidUserId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.RemoveAsync(0, 1, 1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("User id must be greater than 0");
    }

    [Fact]
    public async Task RemoveAsync_WithNonExistentFavorite_ThrowsNotFoundException()
    {
        // Arrange
        _mockFavoriteRepository.Setup(x => x.GetByUserAndEntityAsync(1, 1, 1))
            .ReturnsAsync((Favorite?)null);

        // Act
        var act = async () => await _service.RemoveAsync(1, 1, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Favorite not found");
    }

    [Fact]
    public async Task RemoveAsync_WhenRepositoryFails_ThrowsNotFoundException()
    {
        // Arrange
        var favorite = new Favorite
        {
            Id = 1,
            UserId = 1,
            EntityTypeId = 1,
            EntityId = 1,
            CreatedAt = DateTime.UtcNow
        };

        _mockFavoriteRepository.Setup(x => x.GetByUserAndEntityAsync(1, 1, 1))
            .ReturnsAsync(favorite);
        _mockFavoriteRepository.Setup(x => x.DeleteByUserAndEntityAsync(1, 1, 1))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _service.RemoveAsync(1, 1, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Failed to remove favorite");
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithValidUserId_ReturnsFavorites()
    {
        // Arrange
        var favorites = new List<Favorite>
        {
            new() { Id = 1, UserId = 1, EntityTypeId = 1, EntityId = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, UserId = 1, EntityTypeId = 1, EntityId = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, UserId = 1, EntityTypeId = 2, EntityId = 1, CreatedAt = DateTime.UtcNow }
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockFavoriteRepository.Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(favorites);

        // Act
        var result = await _service.GetUserFavoritesAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(f => f.UserId.Should().Be(1));
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithInvalidUserId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetUserFavoritesAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("User id must be greater than 0");
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _service.GetUserFavoritesAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithNoFavorites_ReturnsEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockFavoriteRepository.Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(new List<Favorite>());

        // Act
        var result = await _service.GetUserFavoritesAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
