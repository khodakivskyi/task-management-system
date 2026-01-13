using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;

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
        IUserRepository userRepository,
        IEntityTypeRepository entityTypeRepository)
    {
        // Validate UserId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(userId, userRepository, "User");

        // Validate EntityTypeId exists
        await EntityValidationHelper.EnsureEntityExistsAsync(entityTypeId, entityTypeRepository, "Entity type");
    }

    /// <summary>
    /// Validates that the user is the owner of the entity (task or project)
    /// </summary>
    public static async Task ValidateEntityOwnershipAsync(
        int userId,
        int entityTypeId,
        int entityId,
        IEntityTypeRepository entityTypeRepository,
        IRepository<TaskModel> taskRepository,
        IRepository<Project> projectRepository)
    {
        var entityType = await entityTypeRepository.GetByIdAsync(entityTypeId);
        if (entityType == null)
        {
            throw new NotFoundException("Entity type not found");
        }

        switch (entityType.Name.ToLower())
        {
            case "task":
                var task = await taskRepository.GetByIdAsync(entityId);
                if (task == null)
                {
                    throw new NotFoundException("Task not found");
                }
                if (task.OwnerId != userId)
                {
                    throw new UnauthorizedException("You can only add your own tasks to favorites");
                }
                break;

            case "project":
                var project = await projectRepository.GetByIdAsync(entityId);
                if (project == null)
                {
                    throw new NotFoundException("Project not found");
                }
                if (project.OwnerId != userId)
                {
                    throw new UnauthorizedException("You can only add your own projects to favorites");
                }
                break;

            default:
                throw new BadRequestException($"Unsupported entity type: {entityType.Name}");
        }
    }
}
