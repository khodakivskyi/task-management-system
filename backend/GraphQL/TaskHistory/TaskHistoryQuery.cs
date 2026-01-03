using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for TaskHistory
/// </summary>
public class TaskHistoryQuery
{
    public async Task<IEnumerable<TaskHistory>> GetTaskHistoryByTaskId(
        int taskId,
        [Service] ITaskHistoryService taskHistoryService)
    {
        return await taskHistoryService.GetByTaskIdAsync(taskId);
    }
}
