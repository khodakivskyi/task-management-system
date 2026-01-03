using backend.GraphQL.Categories.Inputs;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL Mutation operations for Categories
/// </summary>
public class CategoriesMutation
{
    public async Task<Category> CreateCategory(
        CreateCategoryInput input,
        [Service] ICategoryService categoryService)
    {
        var category = new Category
        {
            Name = input.Name,
            Color = input.Color
        };

        return await categoryService.CreateAsync(category);
    }

    public async Task<Category> UpdateCategory(
        UpdateCategoryInput input,
        [Service] ICategoryService categoryService)
    {
        var category = new Category
        {
            Id = input.Id,
            Name = input.Name,
            Color = input.Color
        };

        return await categoryService.UpdateAsync(category);
    }

    public async Task<bool> DeleteCategory(
        int id,
        [Service] ICategoryService categoryService)
    {
        await categoryService.DeleteAsync(id);
        return true;
    }
}
