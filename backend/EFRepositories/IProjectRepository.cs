using backend.EFModels;

namespace backend.EFRepositories;

/// <summary>
/// Repository interface for Project entity using Entity Framework Core
/// </summary>
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id);
    Task<Project?> GetWithDetailsAsync(int projectId);
    Task<IEnumerable<Project>> GetAllAsync();
    Task<IEnumerable<Project>> GetByOwnerIdAsync(int ownerId);
    Task<Project> CreateAsync(Project project);
    Task<Project> UpdateAsync(Project project);
    Task<bool> DeleteAsync(int id);
}

