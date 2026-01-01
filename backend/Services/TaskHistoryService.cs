using backend.Helpers;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;

namespace backend.Services;

/// <summary>
/// Service for TaskHistory operations with business logic and validation
/// TaskHistory records are automatically created by triggers and cannot be manually created, updated, or deleted
/// </summary>
public class TaskHistoryService : ITaskHistoryService
{
    private readonly TaskHistoryRepository _taskHistoryRepository;
    private readonly IRepository<TaskModel> _taskRepository;

    public TaskHistoryService(
        TaskHistoryRepository taskHistoryRepository,
        IRepository<TaskModel> taskRepository)
    {
        _taskHistoryRepository = taskHistoryRepository ?? throw new ArgumentNullException(nameof(taskHistoryRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
    }

    public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId)
    {
        ValidationHelper.ValidateId(taskId, "Task");
        await EntityValidationHelper.EnsureEntityExistsAsync(taskId, _taskRepository, "Task");
        return await _taskHistoryRepository.GetByTaskIdAsync(taskId);
    }
}
