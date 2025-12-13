using backend.Application.Repositories;
using backend.Models;

namespace backend.Application.Commands.Tasks;

/// <summary>
/// Handler for CreateTaskCommand
/// </summary>
public class CreateTaskHandler
{
    private readonly TaskRepository _taskRepository;
    private readonly string _connectionString;

    public CreateTaskHandler(string connectionString)
    {
        _connectionString = connectionString;
        _taskRepository = new TaskRepository(connectionString);
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

