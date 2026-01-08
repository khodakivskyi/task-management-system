using backend.Models;

namespace backend.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for EntityType entity operations
/// </summary>
public interface IEntityTypeRepository : IRepository<EntityType>
{
    Task<EntityType?> GetByNameAsync(string name);
}
