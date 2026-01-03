using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for Statuses
/// </summary>
public class StatusesQuery
{
    public async Task<IEnumerable<Status>> GetStatuses(
        [Service] IStatusService statusService)
    {
        return await statusService.GetAllAsync();
    }

    public async Task<Status?> GetStatusById(
        int id,
        [Service] IStatusService statusService)
    {
        return await statusService.GetByIdAsync(id);
    }
}
