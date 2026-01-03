using backend.Models.DTO;
using backend.Models.Filters;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for Task Search
/// </summary>
public class TaskSearchQuery
{
    public async Task<IEnumerable<TaskSearchResultDto>> SearchTasks(
        TaskSearchFilter filter,
        [Service] ITaskSearchService taskSearchService)
    {
        return await taskSearchService.SearchTasksAsync(filter);
    }
}
