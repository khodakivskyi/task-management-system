using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;

namespace backend.Services;

/// <summary>
/// Service for Favorite operations with business logic and validation
/// </summary>
public class FavoriteService : IFavoriteService
{
    private readonly FavoriteRepository _favoriteRepository;
    private readonly IRepository<User> _userRepository;
    private readonly EntityTypeRepository _entityTypeRepository;

    public FavoriteService(
        FavoriteRepository favoriteRepository,
        IRepository<User> userRepository,
        EntityTypeRepository entityTypeRepository)
    {
        _favoriteRepository = favoriteRepository ?? throw new ArgumentNullException(nameof(favoriteRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _entityTypeRepository = entityTypeRepository ?? throw new ArgumentNullException(nameof(entityTypeRepository));
    }

    public async Task<Favorite> AddAsync(int userId, int entityTypeId, int entityId)
    {
        if (userId <= 0)
        {
            throw new BadRequestException("User id must be greater than 0");
        }

        if (entityTypeId <= 0)
        {
            throw new BadRequestException("Entity type id must be greater than 0");
        }

        if (entityId <= 0)
        {
            throw new BadRequestException("Entity id must be greater than 0");
        }

        // Validate UserId exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }

        // Validate EntityTypeId exists
        var entityType = await _entityTypeRepository.GetByIdAsync(entityTypeId);
        if (entityType == null)
        {
            throw new NotFoundException($"Entity type with id {entityTypeId} not found");
        }

        // Check if favorite already exists
        var existingFavorite = await _favoriteRepository.GetByUserAndEntityAsync(userId, entityTypeId, entityId);
        if (existingFavorite != null)
        {
            throw new ConflictException($"Favorite already exists for user {userId}, entity type {entityTypeId}, entity {entityId}");
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
        if (userId <= 0)
        {
            throw new BadRequestException("User id must be greater than 0");
        }

        if (entityTypeId <= 0)
        {
            throw new BadRequestException("Entity type id must be greater than 0");
        }

        if (entityId <= 0)
        {
            throw new BadRequestException("Entity id must be greater than 0");
        }

        // Check if favorite exists
        var favorite = await _favoriteRepository.GetByUserAndEntityAsync(userId, entityTypeId, entityId);
        if (favorite == null)
        {
            throw new NotFoundException($"Favorite not found for user {userId}, entity type {entityTypeId}, entity {entityId}");
        }

        var removed = await _favoriteRepository.DeleteByUserAndEntityAsync(userId, entityTypeId, entityId);
        if (!removed)
        {
            throw new NotFoundException($"Failed to remove favorite for user {userId}, entity type {entityTypeId}, entity {entityId}");
        }
    }

    public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new BadRequestException("User id must be greater than 0");
        }

        // Validate UserId exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }

        return await _favoriteRepository.GetByUserIdAsync(userId);
    }
}
