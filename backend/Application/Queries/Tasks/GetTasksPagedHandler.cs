using backend.Application.QueryRepositories;
using backend.Models;

namespace backend.Application.Queries.Tasks;

/// <summary>
/// Handler for GetTasksPagedQuery
/// </summary>
public class GetTasksPagedHandler
{
    private readonly TaskQueryRepository _taskQueryRepository;
    private readonly string _connectionString;

    public GetTasksPagedHandler(string connectionString)
    {
        _connectionString = connectionString;
        _taskQueryRepository = new TaskQueryRepository(connectionString);
    }

    public async Task<IEnumerable<TaskWithDetailsDto>> HandleAsync(GetTasksPagedQuery query)
    {
        return await _taskQueryRepository.GetPagedAsync(
            query.PageNumber,
            query.PageSize,
            query.SortBy,
            query.SortDirection,
            query.FilterValue
        );
    }
}

