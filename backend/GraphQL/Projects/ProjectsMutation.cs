using backend.GraphQL.Projects.Inputs;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Mutations;

/// <summary>
/// GraphQL Mutation operations for Projects
/// </summary>
public class ProjectsMutation
{
    public async Task<Project> CreateProject(
        CreateProjectInput input,
        [Service] IProjectService projectService)
    {
        var project = new Project
        {
            OwnerId = input.OwnerId,
            Name = input.Name,
            Description = input.Description,
            StartDate = input.StartDate,
            EndDate = input.EndDate
        };

        return await projectService.CreateAsync(project);
    }

    public async Task<Project> UpdateProject(
        UpdateProjectInput input,
        [Service] IProjectService projectService)
    {
        var project = new Project
        {
            Id = input.Id,
            OwnerId = input.OwnerId,
            Name = input.Name,
            Description = input.Description,
            StartDate = input.StartDate,
            EndDate = input.EndDate
        };

        return await projectService.UpdateAsync(project);
    }

    public async Task<bool> DeleteProject(
        DeleteProjectInput input,
        [Service] IProjectService projectService)
    {
        await projectService.DeleteAsync(input.Id, input.OwnerId);
        return true;
    }
}
