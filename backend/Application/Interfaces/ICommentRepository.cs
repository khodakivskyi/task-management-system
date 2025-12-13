using Npgsql;

namespace backend.Application.Interfaces;

/// <summary>
/// Interface for Comment repository operations with transaction support
/// </summary>
public interface ICommentRepository
{
    Task<bool> DeleteByTaskIdAsync(int taskId, NpgsqlTransaction? transaction = null);
}

