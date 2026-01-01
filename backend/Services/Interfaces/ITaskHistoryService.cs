using backend.Models;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for TaskHistory operations
/// </summary>
public interface ITaskHistoryService
{
    Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId);
}
