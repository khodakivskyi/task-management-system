using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for Favorites
/// </summary>
public class FavoritesQuery
{
    public async Task<IEnumerable<Favorite>> GetUserFavorites(
        int userId,
        [Service] IFavoriteService favoriteService)
    {
        return await favoriteService.GetUserFavoritesAsync(userId);
    }
}
