using backend.Application.Interfaces;
using backend.Models;

namespace backend.Application.Commands.Tasks;

/// <summary>
/// Handler for UpdateTaskCommand
/// </summary>
public class UpdateTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskQueryRepository _taskQueryRepository;

    public UpdateTaskHandler(ITaskRepository taskRepository, ITaskQueryRepository taskQueryRepository)
    {
        _taskRepository = taskRepository;
        _taskQueryRepository = taskQueryRepository;
    }

    public async Task<bool> HandleAsync(UpdateTaskCommand command)
    {
        var existingTask = await _taskQueryRepository.GetByIdAsync(command.Id);
        if (existingTask == null)
        {
            throw new ArgumentException($"Task with Id {command.Id} does not exist", nameof(command));
        }

        var task = new TaskModel
        {
            Id = command.Id,
            OwnerId = existingTask.OwnerId,
            StatusId = command.StatusId,
            CategoryId = command.CategoryId,
            ProjectId = command.ProjectId,
            Title = command.Title,
            Description = command.Description,
            Priority = command.Priority,
            Deadline = command.Deadline,
            CreatedAt = existingTask.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            EstimatedHours = command.EstimatedHours,
            ActualHours = command.ActualHours
        };

        return await _taskRepository.UpdateAsync(task);
    }
}

