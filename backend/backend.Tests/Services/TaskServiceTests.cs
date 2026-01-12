using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Models.DTO;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRepository<Status>> _mockStatusRepository;
    private readonly Mock<IRepository<Category>> _mockCategoryRepository;
    private readonly Mock<IRepository<Project>> _mockProjectRepository;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockStatusRepository = new Mock<IRepository<Status>>();
        _mockCategoryRepository = new Mock<IRepository<Category>>();
        _mockProjectRepository = new Mock<IRepository<Project>>();
        _taskService = new TaskService(
            _mockTaskRepository.Object,
            _mockUserRepository.Object,
            _mockStatusRepository.Object,
            _mockCategoryRepository.Object,
            _mockProjectRepository.Object);
    }

    [Fact]
    public async Task GetPagedAsync_WithDefaultParameters_ReturnsPagedTasks()
    {
        // Arrange
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Task 1", "Description 1", 3, DateTime.UtcNow.AddDays(7),
                DateTime.UtcNow, DateTime.UtcNow, 8, 0, "To Do", "#FF0000", "Bug", "#FF5733",
                "John", "Doe", "johndoe"),
            new(2, 1, 2, 1, 1, "Task 2", "Description 2", 5, DateTime.UtcNow.AddDays(3),
                DateTime.UtcNow, DateTime.UtcNow, 16, 4, "In Progress", "#00FF00", "Feature", "#33FF57",
                "Jane", "Smith", "janesmith")
        };

        _mockTaskRepository.Setup(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _taskService.GetPagedAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Task 1");
        result.Should().Contain(t => t.Title == "Task 2");
        _mockTaskRepository.Verify(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithCustomPageNumber_ReturnsCorrectPage()
    {
        // Arrange
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(3, 2, 1, 1, 1, "Task 3", "Description 3", 2, DateTime.UtcNow.AddDays(10),
                DateTime.UtcNow, DateTime.UtcNow, 5, 2, "Done", "#0000FF", "Task", "#5733FF",
                "Bob", "Johnson", "bobjohnson")
        };

        _mockTaskRepository.Setup(x => x.GetPagedAsync(2, 10, "CreatedAt", "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _taskService.GetPagedAsync(pageNumber: 2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Task 3");
        _mockTaskRepository.Verify(x => x.GetPagedAsync(2, 10, "CreatedAt", "DESC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithCustomPageSize_ReturnsCorrectNumberOfTasks()
    {
        // Arrange
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Task 1", "Description 1", 3, DateTime.UtcNow.AddDays(7),
                DateTime.UtcNow, DateTime.UtcNow, 8, 0, "To Do", "#FF0000", "Bug", "#FF5733",
                "John", "Doe", "johndoe"),
            new(2, 1, 2, 1, 1, "Task 2", "Description 2", 5, DateTime.UtcNow.AddDays(3),
                DateTime.UtcNow, DateTime.UtcNow, 16, 4, "In Progress", "#00FF00", "Feature", "#33FF57",
                "Jane", "Smith", "janesmith"),
            new(3, 2, 1, 1, 1, "Task 3", "Description 3", 2, DateTime.UtcNow.AddDays(10),
                DateTime.UtcNow, DateTime.UtcNow, 5, 2, "Done", "#0000FF", "Task", "#5733FF",
                "Bob", "Johnson", "bobjohnson"),
            new(4, 1, 1, 1, 1, "Task 4", "Description 4", 4, DateTime.UtcNow.AddDays(5),
                DateTime.UtcNow, DateTime.UtcNow, 12, 6, "To Do", "#FF0000", "Enhancement", "#FF8C33",
                "Alice", "Williams", "alicewilliams"),
            new(5, 2, 2, 1, 1, "Task 5", "Description 5", 1, DateTime.UtcNow.AddDays(14),
                DateTime.UtcNow, DateTime.UtcNow, 3, 1, "In Progress", "#00FF00", "Bug", "#FF5733",
                "Charlie", "Brown", "charliebrown")
        };

        _mockTaskRepository.Setup(x => x.GetPagedAsync(1, 5, "CreatedAt", "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _taskService.GetPagedAsync(pageSize: 5);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        _mockTaskRepository.Verify(x => x.GetPagedAsync(1, 5, "CreatedAt", "DESC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithSortByTitle_ReturnsSortedTasks()
    {
        // Arrange
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Alpha Task", "Description", 3, DateTime.UtcNow.AddDays(7),
                DateTime.UtcNow, DateTime.UtcNow, 8, 0, "To Do", "#FF0000", "Bug", "#FF5733",
                "John", "Doe", "johndoe"),
            new(2, 1, 2, 1, 1, "Beta Task", "Description", 5, DateTime.UtcNow.AddDays(3),
                DateTime.UtcNow, DateTime.UtcNow, 16, 4, "In Progress", "#00FF00", "Feature", "#33FF57",
                "Jane", "Smith", "janesmith")
        };

        _mockTaskRepository.Setup(x => x.GetPagedAsync(1, 10, "Title", "ASC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _taskService.GetPagedAsync(sortBy: "Title", sortDirection: "ASC");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Alpha Task");
        _mockTaskRepository.Verify(x => x.GetPagedAsync(1, 10, "Title", "ASC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithFilterValue_ReturnsFilteredTasks()
    {
        // Arrange
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Important Task", "Critical bug fix", 5, DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow, DateTime.UtcNow, 8, 0, "To Do", "#FF0000", "Bug", "#FF5733",
                "John", "Doe", "johndoe")
        };

        _mockTaskRepository.Setup(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", "Important"))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _taskService.GetPagedAsync(filterValue: "Important");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Contain("Important");
        _mockTaskRepository.Verify(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", "Important"), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithInvalidPageNumber_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _taskService.GetPagedAsync(pageNumber: 0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Page number must be greater than or equal to 1");
    }

    [Fact]
    public async Task GetPagedAsync_WithNegativePageNumber_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _taskService.GetPagedAsync(pageNumber: -1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Page number must be greater than or equal to 1");
    }

    [Fact]
    public async Task GetPagedAsync_WithInvalidPageSize_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _taskService.GetPagedAsync(pageSize: 0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Page size must be greater than or equal to 1");
    }

    [Fact]
    public async Task GetPagedAsync_WithPageSizeExceedingLimit_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _taskService.GetPagedAsync(pageSize: 101);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Page size cannot exceed 100");
    }

    [Fact]
    public async Task GetPagedAsync_WithInvalidSortColumn_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _taskService.GetPagedAsync(sortBy: "InvalidColumn");

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Invalid sort column");
    }

    [Fact]
    public async Task GetPagedAsync_WithInvalidSortDirection_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _taskService.GetPagedAsync(sortDirection: "INVALID");

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Sort direction must be either 'ASC' or 'DESC'");
    }

    [Fact]
    public async Task GetPagedAsync_WithFilterValueExceedingMaxLength_ThrowsBadRequestException()
    {
        // Arrange
        var longFilterValue = new string('a', 201);

        // Act
        var act = async () => await _taskService.GetPagedAsync(filterValue: longFilterValue);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Filter value cannot exceed 200 characters");
    }

    [Fact]
    public async Task GetPagedAsync_WithAllValidSortColumns_DoesNotThrow()
    {
        // Arrange
        var validColumns = new[] { "Id", "Title", "Priority", "Deadline", "CreatedAt", 
            "UpdatedAt", "EstimatedHours", "ActualHours", "StatusName", "CategoryName", "OwnerName" };
        
        var pagedTasks = new List<TaskWithDetailsDto>();
        
        foreach (var column in validColumns)
        {
            _mockTaskRepository.Setup(x => x.GetPagedAsync(1, 10, column, "DESC", null))
                .ReturnsAsync(pagedTasks);
        }

        // Act & Assert
        foreach (var column in validColumns)
        {
            var act = async () => await _taskService.GetPagedAsync(sortBy: column);
            await act.Should().NotThrowAsync();
        }
    }

    [Fact]
    public async Task GetPagedAsync_WithEmptyResults_ReturnsEmptyList()
    {
        // Arrange
        _mockTaskRepository.Setup(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null))
            .ReturnsAsync(new List<TaskWithDetailsDto>());

        // Act
        var result = await _taskService.GetPagedAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockTaskRepository.Verify(x => x.GetPagedAsync(1, 10, "CreatedAt", "DESC", null), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithAllParameters_PassesThemCorrectly()
    {
        // Arrange
        var pagedTasks = new List<TaskWithDetailsDto>
        {
            new(1, 1, 1, 1, 1, "Task 1", "Description 1", 3, DateTime.UtcNow.AddDays(7),
                DateTime.UtcNow, DateTime.UtcNow, 8, 0, "To Do", "#FF0000", "Bug", "#FF5733",
                "John", "Doe", "johndoe")
        };

        _mockTaskRepository.Setup(x => x.GetPagedAsync(3, 20, "Priority", "ASC", "bug"))
            .ReturnsAsync(pagedTasks);

        // Act
        var result = await _taskService.GetPagedAsync(
            pageNumber: 3, 
            pageSize: 20, 
            sortBy: "Priority", 
            sortDirection: "ASC", 
            filterValue: "bug");

        // Assert
        result.Should().NotBeNull();
        _mockTaskRepository.Verify(x => x.GetPagedAsync(3, 20, "Priority", "ASC", "bug"), Times.Once);
    }

    [Theory]
    [InlineData("asc")]
    [InlineData("ASC")]
    [InlineData("desc")]
    [InlineData("DESC")]
    public async Task GetPagedAsync_WithValidSortDirections_DoesNotThrow(string sortDirection)
    {
        // Arrange
        var pagedTasks = new List<TaskWithDetailsDto>();
        _mockTaskRepository.Setup(x => x.GetPagedAsync(1, 10, "CreatedAt", It.IsAny<string>(), null))
            .ReturnsAsync(pagedTasks);

        // Act
        var act = async () => await _taskService.GetPagedAsync(sortDirection: sortDirection);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData("id")]
    [InlineData("ID")]
    [InlineData("Title")]
    [InlineData("TITLE")]
    [InlineData("priority")]
    public async Task GetPagedAsync_WithCaseInsensitiveSortColumn_DoesNotThrow(string sortBy)
    {
        // Arrange
        var pagedTasks = new List<TaskWithDetailsDto>();
        _mockTaskRepository.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<string>(), "DESC", null))
            .ReturnsAsync(pagedTasks);

        // Act
        var act = async () => await _taskService.GetPagedAsync(sortBy: sortBy);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
