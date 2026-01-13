using backend.Models;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for Category operations (read-only)
/// </summary>
public interface ICategoryService
{
    Task<Category?> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetAllAsync();
}
