using backend.Infrastructure.Repositories.Interfaces;
using backend.Models.DTO;
using backend.Models.Filters;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Provides functionality for searching and retrieving tasks based on specified criteria.
/// </summary>
public class TaskSearchService : ITaskSearchService
{
    private readonly ITaskRepository _taskRepository;
    public TaskSearchService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
    }

    public async Task<IEnumerable<TaskSearchResultDto>> SearchTasksAsync(TaskSearchFilter taskSearchFilter)
    {
        return await _taskRepository.SearchTasksAsync(taskSearchFilter);
    }
}
