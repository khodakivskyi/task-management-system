using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Helpers;

/// <summary>
/// Validation helper for TaskHistory entity
/// </summary>
public static class TaskHistoryHelper
{
    /// <summary>
    /// Validates that a task exists
    /// </summary>
    public static async Task ValidateTaskExistsAsync(
        int taskId,
        IRepository<TaskModel> taskRepository)
    {
        var task = await taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            throw new NotFoundException($"Task with id {taskId} not found");
        }
    }
}
