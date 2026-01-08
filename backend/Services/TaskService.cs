using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Service for Task operations with business logic and validation
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<Status> _statusRepository;
    private readonly IRepository<Category>? _categoryRepository;
    private readonly IRepository<Project>? _projectRepository;

    public TaskService(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
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
        await TaskHelper.ValidateTaskAsync(
            task,
            _userRepository,
            _statusRepository,
            _categoryRepository,
            _projectRepository);

        // Set timestamps
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        var id = await _taskRepository.CreateAsync(task);
        task.Id = id;
        return task;
    }

    public async Task<TaskModel?> GetByIdAsync(int id)
    {
        ValidationHelper.ValidateId(id, "Task");
        return await _taskRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task<TaskModel> UpdateAsync(TaskModel task)
    {
        ValidationHelper.ValidateId(task.Id, "Task");

        // Check if task exists
        var existingTask = await EntityValidationHelper.EnsureEntityExistsAsync(task.Id, _taskRepository, "Task");

        await TaskHelper.ValidateTaskAsync(
            task,
            _userRepository,
            _statusRepository,
            _categoryRepository,
            _projectRepository);

        // Preserve CreatedAt and OwnerId
        task.CreatedAt = existingTask.CreatedAt;
        task.OwnerId = existingTask.OwnerId;
        task.UpdatedAt = DateTime.UtcNow;

        var updated = await _taskRepository.UpdateAsync(task);
        if (!updated)
        {
            throw new NotFoundException("Failed to update task");
        }

        return task;
    }

    public async Task DeleteAsync(int id)
    {
        ValidationHelper.ValidateId(id, "Task");

        await EntityValidationHelper.EnsureEntityExistsAsync(id, _taskRepository, "Task");

        var deleted = await _taskRepository.DeleteAsync(id);
        if (!deleted)
        {
            throw new NotFoundException("Failed to delete task");
        }
    }
}
