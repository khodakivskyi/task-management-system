using backend.EFModels;

namespace backend.EFRepositories;

/// <summary>
/// Repository interface for Project entity using Entity Framework Core
/// </summary>
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default);
    Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

