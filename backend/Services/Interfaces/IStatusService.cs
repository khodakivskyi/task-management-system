using backend.Models;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for Status operations
/// </summary>
public interface IStatusService
{
    Task<IEnumerable<Status>> GetAllAsync();
    Task<Status?> GetByIdAsync(int id);
}
