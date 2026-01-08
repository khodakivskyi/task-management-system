using backend.Models;

namespace backend.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for ProjectMember entity operations
/// </summary>
public interface IProjectMemberRepository : IRepository<ProjectMember>
{
    Task<ProjectMember?> GetByProjectAndUserAsync(int projectId, int userId);
    Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(int projectId);
}
