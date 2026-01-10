using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class TaskHistoryServiceTests
{
    private readonly Mock<ITaskHistoryRepository> _mockHistoryRepository;
    private readonly Mock<IRepository<TaskModel>> _mockTaskRepository;
    private readonly TaskHistoryService _service;

    public TaskHistoryServiceTests()
    {
        _mockHistoryRepository = new Mock<ITaskHistoryRepository>();
        _mockTaskRepository = new Mock<IRepository<TaskModel>>();
        _service = new TaskHistoryService(_mockHistoryRepository.Object, _mockTaskRepository.Object);
    }

    [Fact]
    public async Task GetByTaskIdAsync_WithValidId_ReturnsHistory()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskModel { Id = taskId, Title = "Test Task", OwnerId = 1, StatusId = 1 };
        var history = new List<TaskHistory>
        {
            new() { Id = 1, TaskId = taskId, FieldName = "Status", OldValue = "To Do", NewValue = "In Progress", ChangedAt = DateTime.UtcNow },
            new() { Id = 2, TaskId = taskId, FieldName = "Priority", OldValue = "1", NewValue = "5", ChangedAt = DateTime.UtcNow }
        };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockHistoryRepository.Setup(x => x.GetByTaskIdAsync(taskId))
            .ReturnsAsync(history);

        // Act
        var result = await _service.GetByTaskIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(h => h.FieldName == "Status");
        result.Should().Contain(h => h.FieldName == "Priority");
    }

    [Fact]
    public async Task GetByTaskIdAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByTaskIdAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Task id must be greater than 0");
    }

    [Fact]
    public async Task GetByTaskIdAsync_WithNegativeId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByTaskIdAsync(-1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetByTaskIdAsync_WithNonExistentTask_ThrowsNotFoundException()
    {
        // Arrange
        _mockTaskRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((TaskModel?)null);

        // Act
        var act = async () => await _service.GetByTaskIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Task not found");
    }

    [Fact]
    public async Task GetByTaskIdAsync_WithNoHistory_ReturnsEmptyList()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskModel { Id = taskId, Title = "Test Task", OwnerId = 1, StatusId = 1 };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockHistoryRepository.Setup(x => x.GetByTaskIdAsync(taskId))
            .ReturnsAsync(new List<TaskHistory>());

        // Act
        var result = await _service.GetByTaskIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByTaskIdAsync_WithMultipleChanges_ReturnsAllHistory()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskModel { Id = taskId, Title = "Test Task", OwnerId = 1, StatusId = 1 };
        var history = new List<TaskHistory>
        {
            new() { Id = 1, TaskId = taskId, FieldName = "Status", OldValue = "To Do", NewValue = "In Progress", ChangedAt = DateTime.UtcNow.AddDays(-2) },
            new() { Id = 2, TaskId = taskId, FieldName = "Priority", OldValue = "1", NewValue = "5", ChangedAt = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 3, TaskId = taskId, FieldName = "Status", OldValue = "In Progress", NewValue = "Done", ChangedAt = DateTime.UtcNow }
        };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockHistoryRepository.Setup(x => x.GetByTaskIdAsync(taskId))
            .ReturnsAsync(history);

        // Act
        var result = await _service.GetByTaskIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }
}
