using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;

namespace backend.Services;

/// <summary>
/// Service for TaskHistory operations with business logic and validation
/// </summary>
public class TaskHistoryService : ITaskHistoryService
{
    private readonly TaskHistoryRepository _taskHistoryRepository;
    private readonly IRepository<TaskModel> _taskRepository;
    private readonly IRepository<User> _userRepository;

    public TaskHistoryService(
        TaskHistoryRepository taskHistoryRepository,
        IRepository<TaskModel> taskRepository,
        IRepository<User> userRepository)
    {
        _taskHistoryRepository = taskHistoryRepository ?? throw new ArgumentNullException(nameof(taskHistoryRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<TaskHistory> CreateAsync(int taskId, int userId, string fieldName, string? oldValue, string? newValue)
    {
        if (taskId <= 0)
        {
            throw new BadRequestException("Task id must be greater than 0");
        }

        if (userId <= 0)
        {
            throw new BadRequestException("User id must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ValidationException("Field name is required");
        }

        // Validate TaskId exists
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            throw new NotFoundException($"Task with id {taskId} not found");
        }

        // Validate UserId exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }

        var taskHistory = new TaskHistory
        {
            TaskId = taskId,
            UserId = userId,
            FieldName = fieldName,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedAt = DateTime.UtcNow
        };

        var id = await _taskHistoryRepository.CreateAsync(taskHistory);
        taskHistory.Id = id;
        return taskHistory;
    }

    public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId)
    {
        if (taskId <= 0)
        {
            throw new BadRequestException("Task id must be greater than 0");
        }

        // Validate TaskId exists
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            throw new NotFoundException($"Task with id {taskId} not found");
        }

        return await _taskHistoryRepository.GetByTaskIdAsync(taskId);
    }
}
