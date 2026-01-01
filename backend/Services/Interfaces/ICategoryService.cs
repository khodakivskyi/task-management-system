using backend.Models;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for Category operations
/// </summary>
public interface ICategoryService
{
    Task<Category> CreateAsync(Category category);
    Task<Category?> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(int id);
}
