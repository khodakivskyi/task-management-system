using backend.GraphQL.Queries;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Queries;

public class CommentsQueryTests
{
    private readonly Mock<ICommentService> _mockCommentService;
    private readonly CommentsQuery _query;

    public CommentsQueryTests()
    {
        _mockCommentService = new Mock<ICommentService>();
        _query = new CommentsQuery();
    }

    [Fact]
    public async Task GetComments_ReturnsAllComments()
    {
        // Arrange
        var comments = new List<Comment>
        {
            new() { Id = 1, TaskId = 1, UserId = 1, Content = "Comment 1", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, TaskId = 1, UserId = 2, Content = "Comment 2", CreatedAt = DateTime.UtcNow },
            new() { Id = 3, TaskId = 2, UserId = 1, Content = "Comment 3", CreatedAt = DateTime.UtcNow }
        };

        _mockCommentService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(comments);

        // Act
        var result = await _query.GetComments(_mockCommentService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Content == "Comment 1");
        _mockCommentService.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCommentById_WithValidId_ReturnsComment()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 1,
            TaskId = 1,
            UserId = 1,
            Content = "Test Comment",
            CreatedAt = DateTime.UtcNow
        };

        _mockCommentService.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(comment);

        // Act
        var result = await _query.GetCommentById(1, _mockCommentService.Object);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Content.Should().Be("Test Comment");
        _mockCommentService.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetCommentById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockCommentService.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Comment?)null);

        // Act
        var result = await _query.GetCommentById(999, _mockCommentService.Object);

        // Assert
        result.Should().BeNull();
    }
}
