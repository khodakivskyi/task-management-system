using backend.Application.Interfaces;
using backend.Models;

namespace backend.Application.Commands.Tasks;

/// <summary>
/// Handler for CreateTaskCommand
/// </summary>
public class CreateTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public CreateTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<int> HandleAsync(CreateTaskCommand command)
    {
        var task = new TaskModel
        {
            OwnerId = command.OwnerId,
            StatusId = command.StatusId,
            CategoryId = command.CategoryId,
            ProjectId = command.ProjectId,
            Title = command.Title,
            Description = command.Description,
            Priority = command.Priority,
            Deadline = command.Deadline,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EstimatedHours = command.EstimatedHours,
            ActualHours = command.ActualHours
        };

        return await _taskRepository.CreateAsync(task);
    }
}

