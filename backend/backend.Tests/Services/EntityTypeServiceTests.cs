using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class EntityTypeServiceTests
{
    private readonly Mock<IEntityTypeRepository> _mockRepository;
    private readonly EntityTypeService _service;

    public EntityTypeServiceTests()
    {
        _mockRepository = new Mock<IEntityTypeRepository>();
        _service = new EntityTypeService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntityTypes()
    {
        // Arrange
        var entityTypes = new List<EntityType>
        {
            new() { Id = 1, Name = "task" },
            new() { Id = 2, Name = "project" }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(entityTypes);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.Name == "task");
        result.Should().Contain(e => e.Name == "project");
    }

    [Fact]
    public async Task GetAllAsync_WithNoEntityTypes_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<EntityType>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsEntityType()
    {
        // Arrange
        var entityType = new EntityType
        {
            Id = 1,
            Name = "task"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(entityType);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("task");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByIdAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Entity type id must be greater than 0");
    }

    [Fact]
    public async Task GetByIdAsync_WithNegativeId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByIdAsync(-1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((EntityType?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_WithValidName_ReturnsEntityType()
    {
        // Arrange
        var entityType = new EntityType
        {
            Id = 1,
            Name = "task"
        };

        _mockRepository.Setup(x => x.GetByNameAsync("task"))
            .ReturnsAsync(entityType);

        // Act
        var result = await _service.GetByNameAsync("task");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("task");
    }

    [Fact]
    public async Task GetByNameAsync_WithEmptyName_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByNameAsync("");

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Entity type name is required");
    }

    [Fact]
    public async Task GetByNameAsync_WithNullName_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByNameAsync(null!);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Entity type name is required");
    }

    [Fact]
    public async Task GetByNameAsync_WithWhitespaceName_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByNameAsync("   ");

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Entity type name is required");
    }

    [Fact]
    public async Task GetByNameAsync_WithNonExistentName_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByNameAsync("nonexistent"))
            .ReturnsAsync((EntityType?)null);

        // Act
        var result = await _service.GetByNameAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }
}
