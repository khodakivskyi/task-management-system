using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Service for EntityType read-only operations
/// EntityTypes are default values in the database and cannot be created, updated, or deleted
/// </summary>
public class EntityTypeService : IEntityTypeService
{
    private readonly IRepository<EntityType> _entityTypeRepository;

    public EntityTypeService(IRepository<EntityType> entityTypeRepository)
    {
        _entityTypeRepository = entityTypeRepository ?? throw new ArgumentNullException(nameof(entityTypeRepository));
    }

    public async Task<IEnumerable<EntityType>> GetAllAsync()
    {
        return await _entityTypeRepository.GetAllAsync();
    }

    public async Task<EntityType?> GetByIdAsync(int id)
    {
        ValidationHelper.ValidateId(id, "Entity type");
        return await _entityTypeRepository.GetByIdAsync(id);
    }

    public async Task<EntityType?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Entity type name is required");
        }

        return await _entityTypeRepository.GetByNameAsync(name);
    }
}
