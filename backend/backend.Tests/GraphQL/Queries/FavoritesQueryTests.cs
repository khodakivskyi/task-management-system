using backend.GraphQL.Queries;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Queries;

public class FavoritesQueryTests
{
    private readonly Mock<IFavoriteService> _mockFavoriteService;
    private readonly FavoritesQuery _query;

    public FavoritesQueryTests()
    {
        _mockFavoriteService = new Mock<IFavoriteService>();
        _query = new FavoritesQuery();
    }

    [Fact]
    public async Task GetUserFavorites_WithValidUserId_ReturnsFavorites()
    {
        // Arrange
        var userId = 1;
        var favorites = new List<Favorite>
        {
            new() { Id = 1, UserId = userId, EntityTypeId = 1, EntityId = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, UserId = userId, EntityTypeId = 1, EntityId = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, UserId = userId, EntityTypeId = 2, EntityId = 1, CreatedAt = DateTime.UtcNow }
        };

        _mockFavoriteService.Setup(x => x.GetUserFavoritesAsync(userId))
            .ReturnsAsync(favorites);

        // Act
        var result = await _query.GetUserFavorites(userId, _mockFavoriteService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(f => f.UserId.Should().Be(userId));
        _mockFavoriteService.Verify(x => x.GetUserFavoritesAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserFavorites_WithNoFavorites_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        _mockFavoriteService.Setup(x => x.GetUserFavoritesAsync(userId))
            .ReturnsAsync(new List<Favorite>());

        // Act
        var result = await _query.GetUserFavorites(userId, _mockFavoriteService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserFavorites_CallsServiceWithCorrectUserId()
    {
        // Arrange
        var userId = 42;
        _mockFavoriteService.Setup(x => x.GetUserFavoritesAsync(userId))
            .ReturnsAsync(new List<Favorite>());

        // Act
        await _query.GetUserFavorites(userId, _mockFavoriteService.Object);

        // Assert
        _mockFavoriteService.Verify(x => x.GetUserFavoritesAsync(userId), Times.Once);
    }
}
