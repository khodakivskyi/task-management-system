using backend.Models;

namespace backend.Interfaces;

/// <summary>
/// Service interface for TaskHistory operations
/// </summary>
public interface ITaskHistoryService
{
    Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId);
}
