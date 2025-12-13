using backend.Application.Interfaces;
using Npgsql;

namespace backend.Application.Commands.Tasks;

/// <summary>
/// Handler for DeleteTaskCommand
/// </summary>
public class DeleteTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskQueryRepository _taskQueryRepository;
    private readonly ITaskAssigneeRepository _taskAssigneeRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ITaskHistoryRepository _taskHistoryRepository;
    private readonly string _connectionString;

    public DeleteTaskHandler(
        ITaskRepository taskRepository,
        ITaskQueryRepository taskQueryRepository,
        ITaskAssigneeRepository taskAssigneeRepository,
        ICommentRepository commentRepository,
        ITaskHistoryRepository taskHistoryRepository,
        string connectionString)
    {
        _taskRepository = taskRepository;
        _taskQueryRepository = taskQueryRepository;
        _taskAssigneeRepository = taskAssigneeRepository;
        _commentRepository = commentRepository;
        _taskHistoryRepository = taskHistoryRepository;
        _connectionString = connectionString;
    }

    public async Task<bool> HandleAsync(DeleteTaskCommand command)
    {
        var task = await _taskQueryRepository.GetByIdAsync(command.Id);
        if (task == null)
        {
            throw new ArgumentException($"Task with Id {command.Id} does not exist", nameof(command));
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await _taskAssigneeRepository.DeleteByTaskIdAsync(command.Id, transaction);
            await _commentRepository.DeleteByTaskIdAsync(command.Id, transaction);
            await _taskHistoryRepository.DeleteByTaskIdAsync(command.Id, transaction);
            
            var deleted = await _taskRepository.DeleteAsync(command.Id, transaction);
            
            if (!deleted)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

