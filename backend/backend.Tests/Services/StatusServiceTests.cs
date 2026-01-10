using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class StatusServiceTests
{
    private readonly Mock<IRepository<Status>> _mockRepository;
    private readonly StatusService _service;

    public StatusServiceTests()
    {
        _mockRepository = new Mock<IRepository<Status>>();
        _service = new StatusService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllStatuses()
    {
        // Arrange
        var statuses = new List<Status>
        {
            new() { Id = 1, Name = "To Do" },
            new() { Id = 2, Name = "In Progress" },
            new() { Id = 3, Name = "Done" }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(statuses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.Name == "To Do");
        result.Should().Contain(s => s.Name == "In Progress");
        result.Should().Contain(s => s.Name == "Done");
    }

    [Fact]
    public async Task GetAllAsync_WithNoStatuses_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Status>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsStatus()
    {
        // Arrange
        var status = new Status
        {
            Id = 1,
            Name = "To Do"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(status);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("To Do");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByIdAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Status id must be greater than 0");
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
            .ReturnsAsync((Status?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
