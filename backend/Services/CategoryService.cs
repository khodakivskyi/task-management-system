using backend.Exceptions;
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
        // Validation
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            throw new ValidationException("Category name is required");
        }

        if (string.IsNullOrWhiteSpace(category.Color))
        {
            throw new ValidationException("Category color is required");
        }

        // Check for duplicate name
        var existingCategories = await _categoryRepository.GetAllAsync();
        if (existingCategories.Any(c => c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException($"Category with name '{category.Name}' already exists");
        }

        var id = await _categoryRepository.CreateAsync(category);
        category.Id = id;
        return category;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException("Category id must be greater than 0");
        }

        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        if (category.Id <= 0)
        {
            throw new BadRequestException("Category id must be greater than 0");
        }

        // Check if category exists
        var existingCategory = await _categoryRepository.GetByIdAsync(category.Id);
        if (existingCategory == null)
        {
            throw new NotFoundException($"Category with id {category.Id} not found");
        }

        // Validation
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            throw new ValidationException("Category name is required");
        }

        if (string.IsNullOrWhiteSpace(category.Color))
        {
            throw new ValidationException("Category color is required");
        }

        // Check for duplicate name (excluding current category)
        var existingCategories = await _categoryRepository.GetAllAsync();
        if (existingCategories.Any(c => c.Id != category.Id && c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException($"Category with name '{category.Name}' already exists");
        }

        var updated = await _categoryRepository.UpdateAsync(category);
        if (!updated)
        {
            throw new NotFoundException($"Failed to update category with id {category.Id}");
        }

        return category;
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException("Category id must be greater than 0");
        }

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
