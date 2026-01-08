using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Service for Project operations with business logic and validation
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private readonly IUserRepository _userRepository;

    public ProjectService(
        IRepository<Project> projectRepository,
        IUserRepository userRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Project> CreateAsync(Project project)
    {
        await ProjectHelper.ValidateProjectAsync(project, _userRepository);

        var id = await _projectRepository.CreateAsync(project);
        project.Id = id;
        return project;
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        ValidationHelper.ValidateId(id, "Project");
        return await _projectRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _projectRepository.GetAllAsync();
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        ValidationHelper.ValidateId(project.Id, "Project");

        // Check if project exists
        var existingProject = await _projectRepository.GetByIdAsync(project.Id);
        if (existingProject == null)
        {
            throw new NotFoundException($"Project with id {project.Id} not found");
        }

        await ProjectHelper.ValidateProjectAsync(project, _userRepository);

        // Preserve OwnerId
        project.OwnerId = existingProject.OwnerId;

        var updated = await _projectRepository.UpdateAsync(project);
        if (!updated)
        {
            throw new NotFoundException($"Failed to update project with id {project.Id}");
        }

        return project;
    }

    public async Task DeleteAsync(int id)
    {
        ValidationHelper.ValidateId(id, "Project");

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            throw new NotFoundException($"Project with id {id} not found");
        }

        var deleted = await _projectRepository.DeleteAsync(id);
        if (!deleted)
        {
            throw new NotFoundException($"Failed to delete project with id {id}");
        }
    }
}
