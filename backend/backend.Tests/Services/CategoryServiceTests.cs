using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IRepository<Category>> _mockRepository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<IRepository<Category>>();
        _service = new CategoryService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidCategory_ReturnsCreatedCategory()
    {
        // Arrange
        var category = new Category
        {
            Name = "Bug",
            Color = "#FF0000"
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category>());
        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(category);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Bug");
        result.Color.Should().Be("#FF0000");
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ThrowsConflictException()
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
        var act = async () => await _service.CreateAsync(category);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Category with this name already exists");
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ThrowsValidationException()
    {
        // Arrange
        var category = new Category
        {
            Name = "",
            Color = "#FF0000"
        };

        // Act
        var act = async () => await _service.CreateAsync(category);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_WithNullName_ThrowsValidationException()
    {
        // Arrange
        var category = new Category
        {
            Name = null!,
            Color = "#FF0000"
        };

        // Act
        var act = async () => await _service.CreateAsync(category);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCategory()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Bug",
            Color = "#FF0000"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(category);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Bug");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByIdAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Category id must be greater than 0");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Bug", Color = "#FF0000" },
            new() { Id = 2, Name = "Feature", Color = "#00FF00" },
            new() { Id = 3, Name = "Task", Color = "#0000FF" }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Name == "Bug");
    }

    [Fact]
    public async Task GetAllAsync_WithNoCategories_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesCategory()
    {
        // Arrange
        var existingCategory = new Category
        {
            Id = 1,
            Name = "Bug",
            Color = "#FF0000"
        };

        var updatedCategory = new Category
        {
            Id = 1,
            Name = "Critical Bug",
            Color = "#FF00FF"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingCategory);
        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category> { existingCategory });
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAsync(updatedCategory);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Critical Bug");
        result.Color.Should().Be("#FF00FF");
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateName_ThrowsConflictException()
    {
        // Arrange
        var existingCategory = new Category
        {
            Id = 1,
            Name = "Bug",
            Color = "#FF0000"
        };

        var updatedCategory = new Category
        {
            Id = 1,
            Name = "Feature", // This name already exists
            Color = "#FF00FF"
        };

        var allCategories = new List<Category>
        {
            existingCategory,
            new() { Id = 2, Name = "Feature", Color = "#00FF00" }
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingCategory);
        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(allCategories);

        // Act
        var act = async () => await _service.UpdateAsync(updatedCategory);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Category with this name already exists");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var category = new Category
        {
            Id = 999,
            Name = "Bug",
            Color = "#FF0000"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Category?)null);

        // Act
        var act = async () => await _service.UpdateAsync(category);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category not found");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Arrange
        var category = new Category
        {
            Id = 0,
            Name = "Bug",
            Color = "#FF0000"
        };

        // Act
        var act = async () => await _service.UpdateAsync(category);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesCategory()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Bug",
            Color = "#FF0000"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(category);
        _mockRepository.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Category?)null);

        // Act
        var act = async () => await _service.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category not found");
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.DeleteAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryFails_ThrowsNotFoundException()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Bug",
            Color = "#FF0000"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(category);
        _mockRepository.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Failed to delete category");
    }
}
