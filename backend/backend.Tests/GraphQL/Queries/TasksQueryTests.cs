using backend.GraphQL.Queries;
using backend.GraphQL.Tasks.Inputs;
using backend.Models;
using backend.Models.DTO;
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

    [Fact]
    public async Task GetPagedTasks_WithDefaultInput_ReturnsPagedTasks()
    {
        // Arrange
        var input = new GetPagedTasksInput();
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Task 1", "Description 1", 3, DateTime.UtcNow.AddDays(7),
                DateTime.UtcNow, DateTime.UtcNow, 8, 0, "To Do", "#FF0000", "Bug", "#FF5733",
                "John", "Doe", "johndoe"),
            new(2, 1, 2, 1, 1, "Task 2", "Description 2", 5, DateTime.UtcNow.AddDays(3),
                DateTime.UtcNow, DateTime.UtcNow, 16, 4, "In Progress", "#00FF00", "Feature", "#33FF57",
                "Jane", "Smith", "janesmith")
        };

        _mockTaskService.Setup(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Task 1");
        result.Should().Contain(t => t.Title == "Task 2");
        _mockTaskService.Verify(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedTasks_WithCustomPageNumber_ReturnsCorrectPage()
    {
        // Arrange
        var input = new GetPagedTasksInput { PageNumber = 2 };
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(3, 2, 1, 1, 1, "Task 3", "Description 3", 2, DateTime.UtcNow.AddDays(10),
                DateTime.UtcNow, DateTime.UtcNow, 5, 2, "Done", "#0000FF", "Task", "#5733FF",
                "Bob", "Johnson", "bobjohnson")
        };

        _mockTaskService.Setup(x => x.GetPagedAsync(2, 10, "CreatedAt", "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Task 3");
        _mockTaskService.Verify(x => x.GetPagedAsync(2, 10, "CreatedAt", "DESC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedTasks_WithCustomPageSize_ReturnsCorrectNumberOfTasks()
    {
        // Arrange
        var input = new GetPagedTasksInput { PageSize = 5 };
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Task 1", null, null, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john"),
            new(2, 1, 1, 1, 1, "Task 2", null, null, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john"),
            new(3, 1, 1, 1, 1, "Task 3", null, null, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john"),
            new(4, 1, 1, 1, 1, "Task 4", null, null, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john"),
            new(5, 1, 1, 1, 1, "Task 5", null, null, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john")
        };

        _mockTaskService.Setup(x => x.GetPagedAsync(1, 5, "CreatedAt", "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        _mockTaskService.Verify(x => x.GetPagedAsync(1, 5, "CreatedAt", "DESC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedTasks_WithSortByTitle_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var input = new GetPagedTasksInput 
        { 
            SortBy = "Title", 
            SortDirection = "ASC" 
        };
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Alpha Task", null, null, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john"),
            new(2, 1, 1, 1, 1, "Beta Task", null, null, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john")
        };

        _mockTaskService.Setup(x => x.GetPagedAsync(1, 10, "Title", "ASC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Alpha Task");
        _mockTaskService.Verify(x => x.GetPagedAsync(1, 10, "Title", "ASC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedTasks_WithFilterValue_ReturnsFilteredTasks()
    {
        // Arrange
        var input = new GetPagedTasksInput { FilterValue = "Important" };
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Important Task", "Critical bug fix", 5, DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow, DateTime.UtcNow, 8, 0, "To Do", "#FF0000", "Bug", "#FF5733",
                "John", "Doe", "johndoe")
        };

        _mockTaskService.Setup(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", "Important"))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Contain("Important");
        _mockTaskService.Verify(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", "Important"), Times.Once);
    }

    [Fact]
    public async Task GetPagedTasks_WithAllParameters_PassesThemCorrectly()
    {
        // Arrange
        var input = new GetPagedTasksInput
        {
            PageNumber = 3,
            PageSize = 20,
            SortBy = "Priority",
            SortDirection = "ASC",
            FilterValue = "bug"
        };
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Bug Task", "Fix bug", 1, DateTime.UtcNow,
                DateTime.UtcNow, DateTime.UtcNow, 4, 2, "To Do", "#FF0000", "Bug", "#FF5733",
                "John", "Doe", "johndoe")
        };

        _mockTaskService.Setup(x => x.GetPagedAsync(3, 20, "Priority", "ASC", "bug"))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        _mockTaskService.Verify(x => x.GetPagedAsync(3, 20, "Priority", "ASC", "bug"), Times.Once);
    }

    [Fact]
    public async Task GetPagedTasks_WithEmptyResults_ReturnsEmptyList()
    {
        // Arrange
        var input = new GetPagedTasksInput();
        _mockTaskService.Setup(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null))
            .ReturnsAsync(new List<TaskWithDetailsDto>());

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockTaskService.Verify(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedTasks_ReturnsTasksWithAllDetailsFields()
    {
        // Arrange
        var input = new GetPagedTasksInput();
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 10, 2, 3, 4, "Complete Task", "Full description", 5, 
                new DateTime(2025, 12, 31),
                new DateTime(2025, 1, 1),
                new DateTime(2025, 1, 15),
                40, 20, 
                "In Progress", "#00FF00", 
                "Feature", "#33FF57",
                "John", "Doe", "johndoe")
        };

        _mockTaskService.Setup(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var task = result.First();
        task.Id.Should().Be(1);
        task.OwnerId.Should().Be(10);
        task.StatusId.Should().Be(2);
        task.CategoryId.Should().Be(3);
        task.ProjectId.Should().Be(4);
        task.Title.Should().Be("Complete Task");
        task.Description.Should().Be("Full description");
        task.Priority.Should().Be(5);
        task.EstimatedHours.Should().Be(40);
        task.ActualHours.Should().Be(20);
        task.StatusName.Should().Be("In Progress");
        task.StatusColor.Should().Be("#00FF00");
        task.CategoryName.Should().Be("Feature");
        task.CategoryColor.Should().Be("#33FF57");
        task.OwnerName.Should().Be("John");
        task.OwnerSurname.Should().Be("Doe");
        task.OwnerLogin.Should().Be("johndoe");
    }

    [Fact]
    public async Task GetPagedTasks_SortByPriority_ReturnsTasksSortedByPriority()
    {
        // Arrange
        var input = new GetPagedTasksInput 
        { 
            SortBy = "Priority", 
            SortDirection = "DESC" 
        };
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "High Priority", null, 5, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john"),
            new(2, 1, 1, 1, 1, "Medium Priority", null, 3, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john"),
            new(3, 1, 1, 1, 1, "Low Priority", null, 1, null, DateTime.UtcNow, DateTime.UtcNow, 
                0, 0, "To Do", null, null, null, "John", null, "john")
        };

        _mockTaskService.Setup(x => x.GetPagedAsync(1, 10, "Priority", "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _query.GetPagedTasks(input, _mockTaskService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.First().Priority.Should().Be(5);
        result.Last().Priority.Should().Be(1);
        _mockTaskService.Verify(x => x.GetPagedAsync(1, 10, "Priority", "DESC", null), Times.Once);
    }
}
