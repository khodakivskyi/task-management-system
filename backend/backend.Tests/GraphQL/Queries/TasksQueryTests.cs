using backend.GraphQL.Queries;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Queries;

public class TasksQueryTests
{
    private readonly Mock<ITaskService> _mockTaskService;
    private readonly TasksQuery _query;

    public TasksQueryTests()
    {
        _mockTaskService = new Mock<ITaskService>();
        _query = new TasksQuery();
    }

    [Fact]
    public async Task GetTasks_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new List<TaskModel>
        {
            new() { Id = 1, Title = "Task 1", OwnerId = 1, StatusId = 1 },
            new() { Id = 2, Title = "Task 2", OwnerId = 1, StatusId = 2 },
            new() { Id = 3, Title = "Task 3", OwnerId = 2, StatusId = 1 }
        };

        _mockTaskService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(tasks);

        // Act
        var result = await _query.GetTasks(_mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Title == "Task 1");
        result.Should().Contain(t => t.Title == "Task 2");
        result.Should().Contain(t => t.Title == "Task 3");
        _mockTaskService.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTasks_WithNoTasks_ReturnsEmptyList()
    {
        // Arrange
        _mockTaskService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<TaskModel>());

        // Act
        var result = await _query.GetTasks(_mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockTaskService.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTaskById_WithValidId_ReturnsTask()
    {
        // Arrange
        var task = new TaskModel
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            OwnerId = 1,
            StatusId = 1,
            Priority = 3
        };

        _mockTaskService.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(task);

        // Act
        var result = await _query.GetTaskById(1, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Test Task");
        result.Description.Should().Be("Test Description");
        result.Priority.Should().Be(3);
        _mockTaskService.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetTaskById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockTaskService.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((TaskModel?)null);

        // Act
        var result = await _query.GetTaskById(999, _mockTaskService.Object);

        // Assert
        result.Should().BeNull();
        _mockTaskService.Verify(x => x.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task GetTaskById_CallsServiceWithCorrectId()
    {
        // Arrange
        var taskId = 42;
        var task = new TaskModel
        {
            Id = taskId,
            Title = "Specific Task",
            OwnerId = 1,
            StatusId = 1
        };

        _mockTaskService.Setup(x => x.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _query.GetTaskById(taskId, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        _mockTaskService.Verify(x => x.GetByIdAsync(taskId), Times.Once);
    }
}
