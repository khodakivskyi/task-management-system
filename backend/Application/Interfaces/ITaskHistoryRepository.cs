using Npgsql;

namespace backend.Application.Interfaces;

/// <summary>
/// Interface for TaskHistory repository operations with transaction support
/// </summary>
public interface ITaskHistoryRepository
{
    Task<bool> DeleteByTaskIdAsync(int taskId, NpgsqlTransaction? transaction = null);
}

