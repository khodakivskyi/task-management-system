using backend.EFModels;

namespace backend.EFRepositories;

/// <summary>
/// Repository interface for Task entity using Entity Framework Core
/// </summary>
public interface ITaskRepository
{
    Task<TaskModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskModel>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<TaskModel> CreateAsync(TaskModel task, CancellationToken cancellationToken = default);
    Task<TaskModel> UpdateAsync(TaskModel task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

