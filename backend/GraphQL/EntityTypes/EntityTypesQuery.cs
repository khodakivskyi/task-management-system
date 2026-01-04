using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for EntityTypes
/// </summary>
public class EntityTypesQuery
{
    public async Task<IEnumerable<EntityType>> GetEntityTypes(
        [Service] IEntityTypeService entityTypeService)
    {
        return await entityTypeService.GetAllAsync();
    }

    public async Task<EntityType?> GetEntityTypeById(
        int id,
        [Service] IEntityTypeService entityTypeService)
    {
        return await entityTypeService.GetByIdAsync(id);
    }

    public async Task<EntityType?> GetEntityTypeByName(
        string name,
        [Service] IEntityTypeService entityTypeService)
    {
        return await entityTypeService.GetByNameAsync(name);
    }
}
