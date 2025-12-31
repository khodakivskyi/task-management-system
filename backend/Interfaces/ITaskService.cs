using backend.Models;

namespace backend.Interfaces;

/// <summary>
/// Service interface for Task operations
/// </summary>
public interface ITaskService
{
    Task<TaskModel> CreateAsync(TaskModel task);
    Task<TaskModel?> GetByIdAsync(int id);
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task<TaskModel> UpdateAsync(TaskModel task);
    Task DeleteAsync(int id);
}
