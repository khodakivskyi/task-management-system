using backend.GraphQL.Categories.Inputs;
using backend.GraphQL.Mutations;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Mutations;

public class CategoriesMutationTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly CategoriesMutation _mutation;

    public CategoriesMutationTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _mutation = new CategoriesMutation();
    }

    [Fact]
    public async Task CreateCategory_WithValidInput_ReturnsCreatedCategory()
    {
        // Arrange
        var input = new CreateCategoryInput
        {
            Name = "Bug",
            Color = "#FF0000"
        };

        var createdCategory = new Category
        {
            Id = 1,
            Name = input.Name,
            Color = input.Color
        };

        _mockCategoryService.Setup(x => x.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _mutation.CreateCategory(input, _mockCategoryService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Bug");
        result.Color.Should().Be("#FF0000");
        _mockCategoryService.Verify(x => x.CreateAsync(It.Is<Category>(c =>
            c.Name == input.Name &&
            c.Color == input.Color
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_WithValidInput_ReturnsUpdatedCategory()
    {
        // Arrange
        var input = new UpdateCategoryInput
        {
            Id = 1,
            Name = "Critical Bug",
            Color = "#FF00FF"
        };

        var updatedCategory = new Category
        {
            Id = input.Id,
            Name = input.Name,
            Color = input.Color
        };

        _mockCategoryService.Setup(x => x.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(updatedCategory);

        // Act
        var result = await _mutation.UpdateCategory(input, _mockCategoryService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Critical Bug");
        result.Color.Should().Be("#FF00FF");
        _mockCategoryService.Verify(x => x.UpdateAsync(It.Is<Category>(c =>
            c.Id == input.Id &&
            c.Name == input.Name &&
            c.Color == input.Color
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_WithValidId_ReturnsTrue()
    {
        // Arrange
        var categoryId = 1;
        _mockCategoryService.Setup(x => x.DeleteAsync(categoryId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mutation.DeleteCategory(categoryId, _mockCategoryService.Object);

        // Assert
        result.Should().BeTrue();
        _mockCategoryService.Verify(x => x.DeleteAsync(categoryId), Times.Once);
    }
}
