using backend.Application.Interfaces;

namespace backend.Application.Commands.Tasks;

/// <summary>
/// Handler for DeleteTaskCommand
/// </summary>
public class DeleteTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskQueryRepository _taskQueryRepository;

    public DeleteTaskHandler(ITaskRepository taskRepository, ITaskQueryRepository taskQueryRepository)
    {
        _taskRepository = taskRepository;
        _taskQueryRepository = taskQueryRepository;
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

