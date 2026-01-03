using backend.Models;
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
}
