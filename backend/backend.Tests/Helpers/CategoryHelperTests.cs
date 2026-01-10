using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class CategoryHelperTests
{
    private readonly Mock<IRepository<Category>> _mockRepository;

    public CategoryHelperTests()
    {
        _mockRepository = new Mock<IRepository<Category>>();
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithValidCategory_DoesNotThrow()
    {
        // Arrange
        var category = new Category
        {
            Name = "Bug",
            Color = "#FF0000"
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category>());

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object, checkDuplicate: true);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithNullName_ThrowsValidationException()
    {
        // Arrange
        var category = new Category
        {
            Name = null!,
            Color = "#FF0000"
        };

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Category name is required");
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithEmptyName_ThrowsValidationException()
    {
        // Arrange
        var category = new Category
        {
            Name = "",
            Color = "#FF0000"
        };

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Category name is required");
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithWhitespaceName_ThrowsValidationException()
    {
        // Arrange
        var category = new Category
        {
            Name = "   ",
            Color = "#FF0000"
        };

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Category name is required");
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithNullColor_ThrowsValidationException()
    {
        // Arrange
        var category = new Category
        {
            Name = "Bug",
            Color = null!
        };

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Category color is required");
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithEmptyColor_ThrowsValidationException()
    {
        // Arrange
        var category = new Category
        {
            Name = "Bug",
            Color = ""
        };

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Category color is required");
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithDuplicateName_ThrowsConflictException()
    {
        // Arrange
        var category = new Category
        {
            Name = "Bug",
            Color = "#FF0000"
        };

        var existingCategories = new List<Category>
        {
            new() { Id = 1, Name = "Bug", Color = "#00FF00" }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(existingCategories);

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object, checkDuplicate: true);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Category with this name already exists");
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithDuplicateNameCaseInsensitive_ThrowsConflictException()
    {
        // Arrange
        var category = new Category
        {
            Name = "BUG",
            Color = "#FF0000"
        };

        var existingCategories = new List<Category>
        {
            new() { Id = 1, Name = "bug", Color = "#00FF00" }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(existingCategories);

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object, checkDuplicate: true);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Category with this name already exists");
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithDuplicateNameExcludeSelf_DoesNotThrow()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Bug",
            Color = "#FF0000"
        };

        var existingCategories = new List<Category>
        {
            new() { Id = 1, Name = "Bug", Color = "#00FF00" }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(existingCategories);

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object, checkDuplicate: true, excludeId: 1);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateCategoryAsync_WithCheckDuplicateFalse_DoesNotCheckDuplicates()
    {
        // Arrange
        var category = new Category
        {
            Name = "Bug",
            Color = "#FF0000"
        };

        var existingCategories = new List<Category>
        {
            new() { Id = 1, Name = "Bug", Color = "#00FF00" }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(existingCategories);

        // Act
        var act = async () => await CategoryHelper.ValidateCategoryAsync(category, _mockRepository.Object, checkDuplicate: false);

        // Assert
        await act.Should().NotThrowAsync();
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Never);
    }
}
