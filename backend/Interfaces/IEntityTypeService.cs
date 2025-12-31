using backend.Models;

namespace backend.Interfaces;

/// <summary>
/// Service interface for EntityType operations
/// </summary>
public interface IEntityTypeService
{
    Task<IEnumerable<EntityType>> GetAllAsync();
    Task<EntityType?> GetByIdAsync(int id);
    Task<EntityType?> GetByNameAsync(string name);
}
