using backend.GraphQL.Favorites.Inputs;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Mutations;

/// <summary>
/// GraphQL Mutation operations for Favorites
/// </summary>
public class FavoritesMutation
{
    public async Task<Favorite> AddFavorite(
        AddFavoriteInput input,
        [Service] IFavoriteService favoriteService)
    {
        return await favoriteService.AddAsync(input.UserId, input.EntityTypeId, input.EntityId);
    }

    public async Task<bool> RemoveFavorite(
        RemoveFavoriteInput input,
        [Service] IFavoriteService favoriteService)
    {
        await favoriteService.RemoveAsync(input.UserId, input.EntityTypeId, input.EntityId);
        return true;
    }
}
