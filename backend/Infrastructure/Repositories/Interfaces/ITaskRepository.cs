using backend.Models;
using backend.Models.DTO;

namespace backend.Infrastructure.Repositories.Interfaces;

public interface ITaskRepository : IRepository<Models.TaskModel>
{
    // Read operations
    Task<IEnumerable<TaskModel>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<TaskModel>> GetByOwnerIdAsync(int ownerId);
    Task<IEnumerable<TaskWithDetailsDto>> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string sortBy = "CreatedAt",
        string sortDirection = "DESC",
        string? filterValue = null);
    Task<IEnumerable<TaskSearchResultDto>> SearchTasksAsync(TaskSearchFilter taskSearchFilter);
}
