using backend.GraphQL.Queries;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Queries;

public class CategoriesQueryTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly CategoriesQuery _query;

    public CategoriesQueryTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _query = new CategoriesQuery();
    }

    [Fact]
    public async Task GetCategories_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Bug", Color = "#FF0000" },
            new() { Id = 2, Name = "Feature", Color = "#00FF00" },
            new() { Id = 3, Name = "Task", Color = "#0000FF" }
        };

        _mockCategoryService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _query.GetCategories(_mockCategoryService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Name == "Bug");
        result.Should().Contain(c => c.Name == "Feature");
        _mockCategoryService.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCategories_WithNoCategories_ReturnsEmptyList()
    {
        // Arrange
        _mockCategoryService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category>());

        // Act
        var result = await _query.GetCategories(_mockCategoryService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCategoryById_WithValidId_ReturnsCategory()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Bug",
            Color = "#FF0000"
        };

        _mockCategoryService.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(category);

        // Act
        var result = await _query.GetCategoryById(1, _mockCategoryService.Object);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Bug");
        result.Color.Should().Be("#FF0000");
        _mockCategoryService.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetCategoryById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockCategoryService.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _query.GetCategoryById(999, _mockCategoryService.Object);

        // Assert
        result.Should().BeNull();
    }
}
