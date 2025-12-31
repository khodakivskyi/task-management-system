using backend.Interfaces;
using backend.Models.DTO;

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
