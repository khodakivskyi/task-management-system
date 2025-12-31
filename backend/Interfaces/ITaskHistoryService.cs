using backend.Models;

namespace backend.Interfaces;

/// <summary>
/// Service interface for TaskHistory operations
/// </summary>
public interface ITaskHistoryService
{
    Task<TaskHistory> CreateAsync(int taskId, int userId, string fieldName, string? oldValue, string? newValue);
    Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId);
}
