using backend.Exceptions;
using backend.Helpers;
using backend.Interfaces;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Service for Category operations with business logic and validation
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository;

    public CategoryService(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<Category> CreateAsync(Category category)
    {
        await CategoryHelper.ValidateCategoryAsync(category, _categoryRepository, checkDuplicate: true);

        var id = await _categoryRepository.CreateAsync(category);
        category.Id = id;
        return category;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        ValidationHelper.ValidateId(id, "Category");
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        ValidationHelper.ValidateId(category.Id, "Category");

        // Check if category exists
        var existingCategory = await _categoryRepository.GetByIdAsync(category.Id);
        if (existingCategory == null)
        {
            throw new NotFoundException($"Category with id {category.Id} not found");
        }

        await CategoryHelper.ValidateCategoryAsync(category, _categoryRepository, checkDuplicate: true, excludeId: category.Id);

        var updated = await _categoryRepository.UpdateAsync(category);
        if (!updated)
        {
            throw new NotFoundException($"Failed to update category with id {category.Id}");
        }

        return category;
    }

    public async Task DeleteAsync(int id)
    {
        ValidationHelper.ValidateId(id, "Category");

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            throw new NotFoundException($"Category with id {id} not found");
        }

        var deleted = await _categoryRepository.DeleteAsync(id);
        if (!deleted)
        {
            throw new NotFoundException($"Failed to delete category with id {id}");
        }
    }
}
