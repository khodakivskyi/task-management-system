using backend.Models;

namespace backend.Application.Interfaces;

/// <summary>
/// Read-only repository interface for Task queries (CQRS - Queries)
/// </summary>
public interface ITaskQueryRepository
{
    Task<TaskModel?> GetByIdAsync(int id);
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task<IEnumerable<TaskModel>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<TaskModel>> GetByOwnerIdAsync(int ownerId);
    Task<IEnumerable<TaskWithDetailsDto>> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string sortBy = "CreatedAt",
        string sortDirection = "DESC",
        string? filterValue = null);
}

