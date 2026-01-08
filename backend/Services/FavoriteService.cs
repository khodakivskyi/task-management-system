using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Service for Favorite operations with business logic and validation
/// </summary>
public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEntityTypeRepository _entityTypeRepository;

    public FavoriteService(
        IFavoriteRepository favoriteRepository,
        IUserRepository userRepository,
        IEntityTypeRepository entityTypeRepository)
    {
        _favoriteRepository = favoriteRepository ?? throw new ArgumentNullException(nameof(favoriteRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _entityTypeRepository = entityTypeRepository ?? throw new ArgumentNullException(nameof(entityTypeRepository));
    }

    public async Task<Favorite> AddAsync(int userId, int entityTypeId, int entityId)
    {
        ValidationHelper.ValidateId(userId, "User");
        ValidationHelper.ValidateId(entityTypeId, "Entity type");
        ValidationHelper.ValidateId(entityId, "Entity");

        await FavoriteHelper.ValidateFavoriteAsync(userId, entityTypeId, _userRepository, _entityTypeRepository);

        // Check if favorite already exists
        var existingFavorite = await _favoriteRepository.GetByUserAndEntityAsync(userId, entityTypeId, entityId);
        if (existingFavorite != null)
        {
            throw new ConflictException("This item is already in favorites");
        }

        var favorite = new Favorite
        {
            UserId = userId,
            EntityTypeId = entityTypeId,
            EntityId = entityId,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _favoriteRepository.CreateAsync(favorite);
        favorite.Id = id;
        return favorite;
    }

    public async Task RemoveAsync(int userId, int entityTypeId, int entityId)
    {
        ValidationHelper.ValidateId(userId, "User");
        ValidationHelper.ValidateId(entityTypeId, "Entity type");
        ValidationHelper.ValidateId(entityId, "Entity");

        // Check if favorite exists
        var favorite = await _favoriteRepository.GetByUserAndEntityAsync(userId, entityTypeId, entityId);
        if (favorite == null)
        {
            throw new NotFoundException("Favorite not found");
        }

        var removed = await _favoriteRepository.DeleteByUserAndEntityAsync(userId, entityTypeId, entityId);
        if (!removed)
        {
            throw new NotFoundException("Failed to remove favorite");
        }
    }

    public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId)
    {
        ValidationHelper.ValidateId(userId, "User");
        await EntityValidationHelper.EnsureEntityExistsAsync(userId, _userRepository, "User");
        return await _favoriteRepository.GetByUserIdAsync(userId);
    }
}
