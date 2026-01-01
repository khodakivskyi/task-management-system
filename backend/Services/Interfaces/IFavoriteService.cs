using backend.Models;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for Favorite operations
/// </summary>
public interface IFavoriteService
{
    Task<Favorite> AddAsync(int userId, int entityTypeId, int entityId);
    Task RemoveAsync(int userId, int entityTypeId, int entityId);
    Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId);
}
