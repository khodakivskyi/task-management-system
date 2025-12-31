using backend.Models;

namespace backend.Interfaces;

/// <summary>
/// Service interface for Project operations
/// </summary>
public interface IProjectService
{
    Task<Project> CreateAsync(Project project);
    Task<Project?> GetByIdAsync(int id);
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project> UpdateAsync(Project project);
    Task DeleteAsync(int id);
}
