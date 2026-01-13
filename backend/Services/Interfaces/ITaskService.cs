using backend.Models;
using backend.Models.DTO;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for Task operations
/// </summary>
public interface ITaskService
{
    Task<TaskModel> CreateAsync(TaskModel task);
    Task<TaskModel?> GetByIdAsync(int id);
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task<TaskModel> UpdateAsync(TaskModel task);
    Task DeleteAsync(int id, int requestingUserId);
    Task<IEnumerable<TaskWithDetailsDto>> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string sortBy = "CreatedAt",
        string sortDirection = "DESC",
        string? filterValue = null);
}
