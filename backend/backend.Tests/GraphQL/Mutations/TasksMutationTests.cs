using backend.GraphQL.Mutations;
using backend.GraphQL.Tasks.Inputs;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Mutations;

public class TasksMutationTests
{
    private readonly Mock<ITaskService> _mockTaskService;
    private readonly TasksMutation _mutation;

    public TasksMutationTests()
    {
        _mockTaskService = new Mock<ITaskService>();
        _mutation = new TasksMutation();
    }

    [Fact]
    public async Task CreateTask_WithValidInput_ReturnsCreatedTask()
    {
        // Arrange
        var input = new CreateTaskInput
        {
            OwnerId = 1,
            StatusId = 1,
            Title = "New Task",
            Description = "Task Description",
            Priority = 3,
            Deadline = DateTime.UtcNow.AddDays(7),
            EstimatedHours = 8,
            ActualHours = 0
        };

        var createdTask = new TaskModel
        {
            Id = 1,
            OwnerId = input.OwnerId,
            StatusId = input.StatusId,
            Title = input.Title,
            Description = input.Description,
            Priority = input.Priority,
            Deadline = input.Deadline,
            EstimatedHours = input.EstimatedHours,
            ActualHours = input.ActualHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTaskService.Setup(x => x.CreateAsync(It.IsAny<TaskModel>()))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _mutation.CreateTask(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("New Task");
        result.Description.Should().Be("Task Description");
        result.Priority.Should().Be(3);
        _mockTaskService.Verify(x => x.CreateAsync(It.Is<TaskModel>(t =>
            t.OwnerId == input.OwnerId &&
            t.StatusId == input.StatusId &&
            t.Title == input.Title &&
            t.Description == input.Description &&
            t.Priority == input.Priority
        )), Times.Once);
    }

    [Fact]
    public async Task CreateTask_WithOptionalFields_CreatesTaskSuccessfully()
    {
        // Arrange
        var input = new CreateTaskInput
        {
            OwnerId = 1,
            StatusId = 1,
            Title = "Task with Optional Fields",
            CategoryId = 2,
            ProjectId = 3
        };

        var createdTask = new TaskModel
        {
            Id = 1,
            OwnerId = input.OwnerId,
            StatusId = input.StatusId,
            Title = input.Title,
            CategoryId = input.CategoryId,
            ProjectId = input.ProjectId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTaskService.Setup(x => x.CreateAsync(It.IsAny<TaskModel>()))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _mutation.CreateTask(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.CategoryId.Should().Be(2);
        result.ProjectId.Should().Be(3);
        _mockTaskService.Verify(x => x.CreateAsync(It.IsAny<TaskModel>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_WithValidInput_ReturnsUpdatedTask()
    {
        // Arrange
        var input = new UpdateTaskInput
        {
            Id = 1,
            StatusId = 2,
            Title = "Updated Task",
            Description = "Updated Description",
            Priority = 5
        };

        var updatedTask = new TaskModel
        {
            Id = input.Id,
            StatusId = input.StatusId,
            Title = input.Title,
            Description = input.Description,
            Priority = input.Priority,
            OwnerId = 1,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTaskService.Setup(x => x.UpdateAsync(It.IsAny<TaskModel>()))
            .ReturnsAsync(updatedTask);

        // Act
        var result = await _mutation.UpdateTask(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Updated Task");
        result.Description.Should().Be("Updated Description");
        result.StatusId.Should().Be(2);
        result.Priority.Should().Be(5);
        _mockTaskService.Verify(x => x.UpdateAsync(It.Is<TaskModel>(t =>
            t.Id == input.Id &&
            t.StatusId == input.StatusId &&
            t.Title == input.Title &&
            t.Description == input.Description &&
            t.Priority == input.Priority
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_WithOptionalFields_UpdatesSuccessfully()
    {
        // Arrange
        var input = new UpdateTaskInput
        {
            Id = 1,
            StatusId = 1,
            Title = "Updated Task",
            CategoryId = 5,
            ProjectId = 10,
            EstimatedHours = 16
        };

        var updatedTask = new TaskModel
        {
            Id = input.Id,
            StatusId = input.StatusId,
            Title = input.Title,
            CategoryId = input.CategoryId,
            ProjectId = input.ProjectId,
            EstimatedHours = input.EstimatedHours,
            OwnerId = 1,
            UpdatedAt = DateTime.UtcNow
        };

        _mockTaskService.Setup(x => x.UpdateAsync(It.IsAny<TaskModel>()))
            .ReturnsAsync(updatedTask);

        // Act
        var result = await _mutation.UpdateTask(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.CategoryId.Should().Be(5);
        result.ProjectId.Should().Be(10);
        result.EstimatedHours.Should().Be(16);
        _mockTaskService.Verify(x => x.UpdateAsync(It.IsAny<TaskModel>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_WithValidInput_ReturnsTrue()
    {
        // Arrange
        var input = new DeleteTaskInput
        {
            Id = 1,
            OwnerId = 1
        };
        _mockTaskService.Setup(x => x.DeleteAsync(input.Id, input.OwnerId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mutation.DeleteTask(input, _mockTaskService.Object);

        // Assert
        result.Should().BeTrue();
        _mockTaskService.Verify(x => x.DeleteAsync(input.Id, input.OwnerId), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var input = new DeleteTaskInput
        {
            Id = 42,
            OwnerId = 5
        };
        _mockTaskService.Setup(x => x.DeleteAsync(input.Id, input.OwnerId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mutation.DeleteTask(input, _mockTaskService.Object);

        // Assert
        result.Should().BeTrue();
        _mockTaskService.Verify(x => x.DeleteAsync(input.Id, input.OwnerId), Times.Once);
    }
}
