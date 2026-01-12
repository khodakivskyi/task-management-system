using backend.GraphQL.Tasks.Inputs;
using backend.Models;
using backend.Models.DTO;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for Tasks
/// </summary>
public class TasksQuery
{
    public async Task<IEnumerable<TaskModel>> GetTasks(
        [Service] ITaskService taskService)
    {
        return await taskService.GetAllAsync();
    }

    public async Task<TaskModel?> GetTaskById(
        int id,
        [Service] ITaskService taskService)
    {
        return await taskService.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TaskWithDetailsDto>> GetPagedTasks(
        GetPagedTasksInput input,
        [Service] ITaskService taskService)
    {
        return await taskService.GetPagedAsync(
            input.PageNumber,
            input.PageSize,
            input.SortBy,
            input.SortDirection,
            input.FilterValue);
    }
}
