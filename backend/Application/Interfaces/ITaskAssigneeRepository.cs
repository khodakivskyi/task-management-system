using Npgsql;

namespace backend.Application.Interfaces;

/// <summary>
/// Interface for TaskAssignee repository operations with transaction support
/// </summary>
public interface ITaskAssigneeRepository
{
    Task<bool> DeleteByTaskIdAsync(int taskId, NpgsqlTransaction? transaction = null);
}

