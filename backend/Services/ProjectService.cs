using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Service for Project operations with business logic and validation
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<User> _userRepository;

    public ProjectService(
        IRepository<Project> projectRepository,
        IRepository<User> userRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Project> CreateAsync(Project project)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            throw new ValidationException("Project name is required");
        }

        if (project.EndDate < project.StartDate)
        {
            throw new ValidationException("End date must be after start date");
        }

        // Validate OwnerId exists
        var owner = await _userRepository.GetByIdAsync(project.OwnerId);
        if (owner == null)
        {
            throw new NotFoundException($"User with id {project.OwnerId} not found");
        }

        var id = await _projectRepository.CreateAsync(project);
        project.Id = id;
        return project;
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        return await _projectRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _projectRepository.GetAllAsync();
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        if (project.Id <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

        // Check if project exists
        var existingProject = await _projectRepository.GetByIdAsync(project.Id);
        if (existingProject == null)
        {
            throw new NotFoundException($"Project with id {project.Id} not found");
        }

        // Validation
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            throw new ValidationException("Project name is required");
        }

        if (project.EndDate < project.StartDate)
        {
            throw new ValidationException("End date must be after start date");
        }

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
        if (id <= 0)
        {
            throw new BadRequestException("Project id must be greater than 0");
        }

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
