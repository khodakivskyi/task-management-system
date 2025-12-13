using backend.Application.Interfaces;
using backend.Models;

namespace backend.Application.Queries.Tasks;

/// <summary>
/// Handler for GetTasksPagedQuery
/// </summary>
public class GetTasksPagedHandler
{
    private readonly ITaskQueryRepository _taskQueryRepository;

    public GetTasksPagedHandler(ITaskQueryRepository taskQueryRepository)
    {
        _taskQueryRepository = taskQueryRepository;
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

