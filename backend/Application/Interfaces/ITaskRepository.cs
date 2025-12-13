using backend.Models;

namespace backend.Application.Interfaces;

/// <summary>
/// Write-only repository interface for Task entity operations (CQRS - Commands)
/// </summary>
public interface ITaskRepository
{
    Task<int> CreateAsync(TaskModel entity);
    Task<bool> UpdateAsync(TaskModel entity);
    Task<bool> DeleteAsync(int id);
}

