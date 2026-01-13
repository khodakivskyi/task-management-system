using backend.GraphQL.Comments.Inputs;
using backend.GraphQL.Mutations;
using backend.Models;
using backend.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace backend.Tests.GraphQL.Mutations;

public class CommentsMutationTests
{
    private readonly Mock<ICommentService> _mockCommentService;
    private readonly CommentsMutation _mutation;

    public CommentsMutationTests()
    {
        _mockCommentService = new Mock<ICommentService>();
        _mutation = new CommentsMutation();
    }

    [Fact]
    public async Task CreateComment_WithValidInput_ReturnsCreatedComment()
    {
        // Arrange
        var input = new CreateCommentInput
        {
            TaskId = 1,
            UserId = 1,
            Content = "New Comment"
        };

        var createdComment = new Comment
        {
            Id = 1,
            TaskId = input.TaskId,
            UserId = input.UserId,
            Content = input.Content,
            CreatedAt = DateTime.UtcNow
        };

        _mockCommentService.Setup(x => x.CreateAsync(It.IsAny<Comment>()))
            .ReturnsAsync(createdComment);

        // Act
        var result = await _mutation.CreateComment(input, _mockCommentService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Content.Should().Be("New Comment");
        _mockCommentService.Verify(x => x.CreateAsync(It.Is<Comment>(c =>
            c.TaskId == input.TaskId &&
            c.UserId == input.UserId &&
            c.Content == input.Content
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateComment_WithValidInput_ReturnsUpdatedComment()
    {
        // Arrange
        var input = new UpdateCommentInput
        {
            Id = 1,
            UserId = 1,
            Content = "Updated Comment"
        };

        var updatedComment = new Comment
        {
            Id = input.Id,
            UserId = input.UserId,
            Content = input.Content,
            TaskId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockCommentService.Setup(x => x.UpdateAsync(It.IsAny<Comment>()))
            .ReturnsAsync(updatedComment);

        // Act
        var result = await _mutation.UpdateComment(input, _mockCommentService.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Content.Should().Be("Updated Comment");
        _mockCommentService.Verify(x => x.UpdateAsync(It.Is<Comment>(c =>
            c.Id == input.Id &&
            c.UserId == input.UserId &&
            c.Content == input.Content
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteComment_WithValidInput_ReturnsTrue()
    {
        // Arrange
        var input = new DeleteCommentInput
        {
            Id = 1,
            UserId = 1
        };
        _mockCommentService.Setup(x => x.DeleteAsync(input.Id, input.UserId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _mutation.DeleteComment(input, _mockCommentService.Object);

        // Assert
        result.Should().BeTrue();
        _mockCommentService.Verify(x => x.DeleteAsync(input.Id, input.UserId), Times.Once);
    }
}
