using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;

namespace backend.Services;

/// <summary>
/// Service for EntityType read-only operations
/// </summary>
public class EntityTypeService : IEntityTypeService
{
    private readonly EntityTypeRepository _entityTypeRepository;

    public EntityTypeService(EntityTypeRepository entityTypeRepository)
    {
        _entityTypeRepository = entityTypeRepository ?? throw new ArgumentNullException(nameof(entityTypeRepository));
    }

    public async Task<IEnumerable<EntityType>> GetAllAsync()
    {
        return await _entityTypeRepository.GetAllAsync();
    }

    public async Task<EntityType?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException("Entity type id must be greater than 0");
        }

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
