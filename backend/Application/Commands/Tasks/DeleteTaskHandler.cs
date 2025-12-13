using backend.Application.QueryRepositories;
using backend.Application.Repositories;

namespace backend.Application.Commands.Tasks;

/// <summary>
/// Handler for DeleteTaskCommand
/// </summary>
public class DeleteTaskHandler
{
    private readonly TaskRepository _taskRepository;
    private readonly TaskQueryRepository _taskQueryRepository;
    private readonly string _connectionString;

    public DeleteTaskHandler(string connectionString)
    {
        _connectionString = connectionString;
        _taskRepository = new TaskRepository(connectionString);
        _taskQueryRepository = new TaskQueryRepository(connectionString);
    }

    public async Task<bool> HandleAsync(DeleteTaskCommand command)
    {
        var task = await _taskQueryRepository.GetByIdAsync(command.Id);
        if (task == null)
        {
            throw new ArgumentException($"Task with Id {command.Id} does not exist", nameof(command));
        }

        return await _taskRepository.DeleteAsync(command.Id);
    }
}

