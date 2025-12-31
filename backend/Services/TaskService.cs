using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Service for Task operations with business logic and validation
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Status> _statusRepository;
    private readonly IRepository<Category>? _categoryRepository;
    private readonly IRepository<Project>? _projectRepository;

    public TaskService(
        ITaskRepository taskRepository,
        IRepository<User> userRepository,
        IRepository<Status> statusRepository,
        IRepository<Category>? categoryRepository = null,
        IRepository<Project>? projectRepository = null)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _statusRepository = statusRepository ?? throw new ArgumentNullException(nameof(statusRepository));
        _categoryRepository = categoryRepository;
        _projectRepository = projectRepository;
    }

    public async Task<TaskModel> CreateAsync(TaskModel task)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            throw new ValidationException("Task title is required");
        }

        if (task.Priority.HasValue && (task.Priority < 1 || task.Priority > 5))
        {
            throw new ValidationException("Priority must be between 1 and 5");
        }

        // Validate OwnerId exists
        var owner = await _userRepository.GetByIdAsync(task.OwnerId);
        if (owner == null)
        {
            throw new NotFoundException($"User with id {task.OwnerId} not found");
        }

        // Validate StatusId exists
        var status = await _statusRepository.GetByIdAsync(task.StatusId);
        if (status == null)
        {
            throw new NotFoundException($"Status with id {task.StatusId} not found");
        }

        // Validate CategoryId if provided
        if (task.CategoryId.HasValue && _categoryRepository != null)
        {
            var category = await _categoryRepository.GetByIdAsync(task.CategoryId.Value);
            if (category == null)
            {
                throw new NotFoundException($"Category with id {task.CategoryId.Value} not found");
            }
        }

        // Validate ProjectId if provided
        if (task.ProjectId.HasValue && _projectRepository != null)
        {
            var project = await _projectRepository.GetByIdAsync(task.ProjectId.Value);
            if (project == null)
            {
                throw new NotFoundException($"Project with id {task.ProjectId.Value} not found");
            }
        }

        // Set timestamps
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        var id = await _taskRepository.CreateAsync(task);
        task.Id = id;
        return task;
    }

    public async Task<TaskModel?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException("Task id must be greater than 0");
        }

        return await _taskRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task<TaskModel> UpdateAsync(TaskModel task)
    {
        if (task.Id <= 0)
        {
            throw new BadRequestException("Task id must be greater than 0");
        }

        // Check if task exists
        var existingTask = await _taskRepository.GetByIdAsync(task.Id);
        if (existingTask == null)
        {
            throw new NotFoundException($"Task with id {task.Id} not found");
        }

        // Validation
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            throw new ValidationException("Task title is required");
        }

        if (task.Priority.HasValue && (task.Priority < 1 || task.Priority > 5))
        {
            throw new ValidationException("Priority must be between 1 and 5");
        }

        // Validate StatusId exists
        var status = await _statusRepository.GetByIdAsync(task.StatusId);
        if (status == null)
        {
            throw new NotFoundException($"Status with id {task.StatusId} not found");
        }

        // Validate CategoryId if provided
        if (task.CategoryId.HasValue && _categoryRepository != null)
        {
            var category = await _categoryRepository.GetByIdAsync(task.CategoryId.Value);
            if (category == null)
            {
                throw new NotFoundException($"Category with id {task.CategoryId.Value} not found");
            }
        }

        // Validate ProjectId if provided
        if (task.ProjectId.HasValue && _projectRepository != null)
        {
            var project = await _projectRepository.GetByIdAsync(task.ProjectId.Value);
            if (project == null)
            {
                throw new NotFoundException($"Project with id {task.ProjectId.Value} not found");
            }
        }

        // Preserve CreatedAt and OwnerId
        task.CreatedAt = existingTask.CreatedAt;
        task.OwnerId = existingTask.OwnerId;
        task.UpdatedAt = DateTime.UtcNow;

        var updated = await _taskRepository.UpdateAsync(task);
        if (!updated)
        {
            throw new NotFoundException($"Failed to update task with id {task.Id}");
        }

        return task;
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException("Task id must be greater than 0");
        }

        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            throw new NotFoundException($"Task with id {id} not found");
        }

        var deleted = await _taskRepository.DeleteAsync(id);
        if (!deleted)
        {
            throw new NotFoundException($"Failed to delete task with id {id}");
        }
    }
}
