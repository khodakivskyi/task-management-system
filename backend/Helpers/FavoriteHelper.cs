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
        EntityTypeRepository entityTypeRepository)
    {
        // Validate UserId exists
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }

        // Validate EntityTypeId exists
        var entityType = await entityTypeRepository.GetByIdAsync(entityTypeId);
        if (entityType == null)
        {
            throw new NotFoundException($"Entity type with id {entityTypeId} not found");
        }
    }
}
