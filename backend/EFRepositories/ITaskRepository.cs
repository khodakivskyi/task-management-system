using backend.EFModels;
using backend.EFRepositories.DTOs;

namespace backend.EFRepositories;

/// <summary>
/// Repository interface for Task entity using Entity Framework Core
/// </summary>
public interface ITaskRepository
{
    // Basic CRUD operations
    Task<TaskModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskModel>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<TaskModel> CreateAsync(TaskModel task, CancellationToken cancellationToken = default);
    Task<TaskModel> UpdateAsync(TaskModel task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    // Eager loading with Include/ThenInclude
    Task<TaskModel?> GetByIdWithRelationsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskModel>> GetAllWithRelationsAsync(bool trackChanges = false, CancellationToken cancellationToken = default);

    // Projection to DTO
    Task<IEnumerable<TaskDto>> GetTasksAsDtoAsync(CancellationToken cancellationToken = default);

    // Pagination
    Task<PagedResult<TaskModel>> GetPagedAsync(int pageNumber, int pageSize, string? sortBy = null, string? sortDirection = "asc", CancellationToken cancellationToken = default);

    // Grouping with aggregation
    Task<IEnumerable<TaskGroupByStatusDto>> GetTasksGroupedByStatusAsync(CancellationToken cancellationToken = default);

    // Complex filtering
    Task<IEnumerable<TaskModel>> GetFilteredTasksAsync(
        int? ownerId = null,
        int? statusId = null,
        int? projectId = null,
        int? minPriority = null,
        int? maxPriority = null,
        DateTime? deadlineFrom = null,
        DateTime? deadlineTo = null,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);
}

