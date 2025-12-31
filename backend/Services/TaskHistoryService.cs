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

    public TaskHistoryService(
        TaskHistoryRepository taskHistoryRepository,
        IRepository<TaskModel> taskRepository )
    {
        _taskHistoryRepository = taskHistoryRepository ?? throw new ArgumentNullException(nameof(taskHistoryRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
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
