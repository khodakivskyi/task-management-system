using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;

namespace backend.Helpers;

/// <summary>
/// Validation helper for Favorite entity
/// </summary>
public static class FavoriteHelper
{
    /// <summary>
    /// Validates favorite data
    /// </summary>
    public static async Task ValidateFavoriteAsync(
        int userId,
        int entityTypeId,
        IRepository<User> userRepository,
        IRepository<EntityType> entityTypeRepository)
    {
        // Validate UserId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(userId, userRepository, "User");

        // Validate EntityTypeId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(entityTypeId, entityTypeRepository, "Entity type");
    }
}
