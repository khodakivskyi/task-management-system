using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;

namespace backend.Helpers;

/// <summary>
/// Helper for validating entity existence
/// </summary>
public static class EntityValidationHelper
{
    /// <summary>
    /// Gets an entity by ID and throws NotFoundException if not found
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entityId">ID of the entity to get</param>
    /// <param name="repository">Repository to get entity from</param>
    /// <param name="entityName">Name of the entity for error message (e.g., "User", "Task")</param>
    public static async Task<T> EnsureEntityExistsAsync<T>(
        int entityId,
        IRepository<T> repository,
        string entityName) where T : class
    {
        var entity = await repository.GetByIdAsync(entityId);
        if (entity == null)
        {
            throw new NotFoundException($"{entityName} not found");
        }
        return entity;
    }

    /// <summary>
    /// Ensures that an entity exists in the repository if the ID is provided, throws NotFoundException if not found
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entityId">Optional ID of the entity to check</param>
    /// <param name="repository">Repository to check entity existence</param>
    /// <param name="entityName">Name of the entity for error message (e.g., "Category", "Project")</param>
    public static async Task EnsureEntityExistsIfProvidedAsync<T>(
        int? entityId,
        IRepository<T>? repository,
        string entityName) where T : class
    {
        if (entityId.HasValue && repository != null)
        {
            await EnsureEntityExistsAsync(entityId.Value, repository, entityName);
        }
    }
}
