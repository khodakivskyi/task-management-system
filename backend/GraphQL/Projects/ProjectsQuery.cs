using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for Projects
/// </summary>
public class ProjectsQuery
{
    public async Task<IEnumerable<Project>> GetProjects(
        [Service] IProjectService projectService)
    {
        return await projectService.GetAllAsync();
    }

    public async Task<Project?> GetProjectById(
        int id,
        [Service] IProjectService projectService)
    {
        return await projectService.GetByIdAsync(id);
    }
}
