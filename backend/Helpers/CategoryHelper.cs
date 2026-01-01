using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;

namespace backend.Helpers;

/// <summary>
/// Validation helper for Category entity
/// </summary>
public static class CategoryHelper
{
    /// <summary>
    /// Validates a category entity
    /// </summary>
    public static async Task ValidateCategoryAsync(
        Category category,
        IRepository<Category> categoryRepository,
        bool checkDuplicate = false,
        int? excludeId = null)
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
        if (checkDuplicate)
        {
            var existingCategories = await categoryRepository.GetAllAsync();
            if (existingCategories.Any(c =>
                (excludeId == null || c.Id != excludeId.Value) &&
                c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException($"Category with name '{category.Name}' already exists");
            }
        }
    }
}
