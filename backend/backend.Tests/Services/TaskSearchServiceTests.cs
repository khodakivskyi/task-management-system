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
            new() { Id = 1, Title = "Task 1", StatusName = "To Do" },
            new() { Id = 2, Title = "Task 2", StatusName = "In Progress" },
            new() { Id = 3, Title = "Task 3", StatusName = "Done" }
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
            new() { Id = 1, Title = "Important Task", StatusName = "To Do" }
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
            new() { Id = 1, Title = "Task 1", StatusName = "To Do" },
            new() { Id = 2, Title = "Task 2", StatusName = "To Do" }
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
            new() { Id = 1, Title = "High Priority Task", Priority = 5, StatusName = "To Do" },
            new() { Id = 2, Title = "Medium Priority Task", Priority = 3, StatusName = "Done" }
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
            new() { Id = 1, Title = "Low Priority Task", Priority = 1, StatusName = "To Do" },
            new() { Id = 2, Title = "Medium Priority Task", Priority = 3, StatusName = "Done" }
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
            new() { Id = 1, Title = "Important Task", StatusName = "To Do", Priority = 5 }
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
            new() { Id = 1, Title = "Task 1", StatusName = "To Do" },
            new() { Id = 2, Title = "Task 2", StatusName = "Done" }
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
            new() { Id = 1, Title = "Task 1", StatusName = "To Do" }
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
            new() { Id = 1, Title = "Task 1", Priority = 2, StatusName = "To Do" },
            new() { Id = 2, Title = "Task 2", Priority = 3, StatusName = "Done" },
            new() { Id = 3, Title = "Task 3", Priority = 4, StatusName = "In Progress" }
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
