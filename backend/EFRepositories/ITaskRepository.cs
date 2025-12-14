using backend.EFModels;

namespace backend.EFRepositories;

/// <summary>
/// Repository interface for Task entity using Entity Framework Core
/// </summary>
public interface ITaskRepository
{
    Task<TaskModel?> GetByIdAsync(int id);
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task<IEnumerable<TaskModel>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<TaskModel>> GetByOwnerIdAsync(int ownerId);
    Task<TaskModel> CreateAsync(TaskModel task);
    Task<TaskModel> UpdateAsync(TaskModel task);
    Task<bool> DeleteAsync(int id);
    Task<int> CountAsync();
}

