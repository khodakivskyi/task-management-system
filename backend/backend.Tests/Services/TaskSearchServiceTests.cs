using backend.Infrastructure.Repositories.Interfaces;
using backend.Models.DTO;
using backend.Models.Filters;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class TaskSearchServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly TaskSearchService _service;

    public TaskSearchServiceTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _service = new TaskSearchService(_mockRepository.Object);
    }

    [Fact]
    public async Task SearchTasksAsync_WithNoFilter_ReturnsAllTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter();
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Task 1", "To Do", null, null),
            new(2, "Task 2", "In Progress", null, null),
            new(3, "Task 3", "Done", null, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Title == "Task 1");
    }

    [Fact]
    public async Task SearchTasksAsync_WithSearchTextFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter { SearchText = "Important" };
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Important Task", "To Do", null, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Contain("Important");
    }

    [Fact]
    public async Task SearchTasksAsync_WithStatusFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter { StatusId = 1 };
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Task 1", "To Do", null, null),
            new(2, "Task 2", "To Do", null, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.StatusName.Should().Be("To Do"));
    }

    [Fact]
    public async Task SearchTasksAsync_WithPriorityMinFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter { PriorityMin = 3 };
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "High Priority Task", "To Do", 5, null),
            new(2, "Medium Priority Task", "Done", 3, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.Priority.Should().BeGreaterOrEqualTo(3));
    }

    [Fact]
    public async Task SearchTasksAsync_WithPriorityMaxFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter { PriorityMax = 3 };
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Low Priority Task", "To Do", 1, null),
            new(2, "Medium Priority Task", "Done", 3, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.Priority.Should().BeLessOrEqualTo(3));
    }

    [Fact]
    public async Task SearchTasksAsync_WithMultipleFilters_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter
        {
            SearchText = "Important",
            StatusId = 1,
            PriorityMin = 3,
            PriorityMax = 5
        };
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Important Task", "To Do", 5, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Contain("Important");
        result.First().Priority.Should().Be(5);
    }

    [Fact]
    public async Task SearchTasksAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var filter = new TaskSearchFilter { SearchText = "NonExistent" };
        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(new List<TaskSearchResultDto>());

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchTasksAsync_WithUserIdFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter { UserId = 1 };
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Task 1", "To Do", null, null),
            new(2, "Task 2", "Done", null, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchTasksAsync_WithProjectIdFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter { ProjectId = 1 };
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Task 1", "To Do", null, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchTasksAsync_WithPriorityRangeFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter { PriorityMin = 2, PriorityMax = 4 };
        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Task 1", "To Do", 2, null),
            new(2, "Task 2", "Done", 3, null),
            new(3, "Task 3", "In Progress", 4, null)
        };

        _mockRepository.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _service.SearchTasksAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(t =>
        {
            t.Priority.Should().BeGreaterOrEqualTo(2);
            t.Priority.Should().BeLessOrEqualTo(4);
        });
    }
}
