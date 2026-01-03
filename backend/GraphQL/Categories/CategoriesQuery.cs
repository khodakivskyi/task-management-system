using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Query operations for Categories
/// </summary>
public class CategoriesQuery
{
    public async Task<IEnumerable<Category>> GetCategories(
        [Service] ICategoryService categoryService)
    {
        return await categoryService.GetAllAsync();
    }

    public async Task<Category?> GetCategoryById(
        int id,
        [Service] ICategoryService categoryService)
    {
        return await categoryService.GetByIdAsync(id);
    }
}
