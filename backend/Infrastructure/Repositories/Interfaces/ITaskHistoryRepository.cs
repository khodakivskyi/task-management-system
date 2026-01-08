using backend.Models;
using Npgsql;

namespace backend.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for TaskHistory entity operations
/// </summary>
public interface ITaskHistoryRepository : IRepository<TaskHistory>
{
    Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId);
}
