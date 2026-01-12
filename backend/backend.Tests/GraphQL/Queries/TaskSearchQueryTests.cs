using backend.GraphQL.Queries;
using backend.Models.DTO;
using backend.Models.Filters;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Queries;

public class TaskSearchQueryTests
{
    private readonly Mock<ITaskSearchService> _mockTaskSearchService;
    private readonly TaskSearchQuery _query;

    public TaskSearchQueryTests()
    {
        _mockTaskSearchService = new Mock<ITaskSearchService>();
        _query = new TaskSearchQuery();
    }

    [Fact]
    public async Task SearchTasks_WithFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filter = new TaskSearchFilter
        {
            SearchText = "Important",
            StatusId = 1
        };

        var searchResults = new List<TaskSearchResultDto>
        {
            new(1, "Important Task 1", "To Do", null, null),
            new(2, "Important Task 2", "To Do", null, null)
        };

        _mockTaskSearchService.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _query.SearchTasks(filter, _mockTaskSearchService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.Title.Should().Contain("Important"));
        _mockTaskSearchService.Verify(x => x.SearchTasksAsync(filter), Times.Once);
    }

    [Fact]
    public async Task SearchTasks_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var filter = new TaskSearchFilter { SearchText = "NonExistent" };
        _mockTaskSearchService.Setup(x => x.SearchTasksAsync(filter))
            .ReturnsAsync(new List<TaskSearchResultDto>());

        // Act
        var result = await _query.SearchTasks(filter, _mockTaskSearchService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
