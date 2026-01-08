using backend.Models;

namespace backend.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for Favorite entity operations
/// </summary>
public interface IFavoriteRepository : IRepository<Favorite>
{
    Task<Favorite?> GetByUserAndEntityAsync(int userId, int entityTypeId, int entityId);
    Task<IEnumerable<Favorite>> GetByUserIdAsync(int userId);
    Task<bool> DeleteByUserAndEntityAsync(int userId, int entityTypeId, int entityId);
}
