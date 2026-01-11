using backend.GraphQL.Favorites.Inputs;
using backend.GraphQL.Mutations;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Mutations;

public class FavoritesMutationTests
{
    private readonly Mock<IFavoriteService> _mockFavoriteService;
    private readonly FavoritesMutation _mutation;

    public FavoritesMutationTests()
    {
        _mockFavoriteService = new Mock<IFavoriteService>();
        _mutation = new FavoritesMutation();
    }

    [Fact]
    public async Task AddFavorite_WithValidInput_ReturnsCreatedFavorite()
    {
        // Arrange
        var input = new AddFavoriteInput
        {
            UserId = 1,
            EntityTypeId = 1,
            EntityId = 1
        };

        var createdFavorite = new Favorite
        {
            Id = 1,
            UserId = input.UserId,
            EntityTypeId = input.EntityTypeId,
            EntityId = input.EntityId,
            CreatedAt = DateTime.UtcNow
        };

        _mockFavoriteService.Setup(x => x.AddAsync(input.UserId, input.EntityTypeId, input.EntityId))
            .ReturnsAsync(createdFavorite);

        // Act
        var result = await _mutation.AddFavorite(input, _mockFavoriteService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.UserId.Should().Be(input.UserId);
        result.EntityTypeId.Should().Be(input.EntityTypeId);
        result.EntityId.Should().Be(input.EntityId);
        _mockFavoriteService.Verify(x => x.AddAsync(input.UserId, input.EntityTypeId, input.EntityId), Times.Once);
    }

    [Fact]
    public async Task RemoveFavorite_WithValidInput_ReturnsTrue()
    {
        // Arrange
        var input = new RemoveFavoriteInput
        {
            UserId = 1,
            EntityTypeId = 1,
            EntityId = 1
        };

        _mockFavoriteService.Setup(x => x.RemoveAsync(input.UserId, input.EntityTypeId, input.EntityId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mutation.RemoveFavorite(input, _mockFavoriteService.Object);

        // Assert
        result.Should().BeTrue();
        _mockFavoriteService.Verify(x => x.RemoveAsync(input.UserId, input.EntityTypeId, input.EntityId), Times.Once);
    }
}
