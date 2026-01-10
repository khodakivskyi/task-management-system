using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class CommentHelperTests
{
    private readonly Mock<IRepository<TaskModel>> _mockTaskRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public CommentHelperTests()
    {
        _mockTaskRepository = new Mock<IRepository<TaskModel>>();
        _mockUserRepository = new Mock<IUserRepository>();
    }

    [Fact]
    public async Task ValidateCommentAsync_WithValidComment_DoesNotThrow()
    {
        // Arrange
        var comment = new Comment
        {
            TaskId = 1,
            UserId = 1,
            Content = "Test comment"
        };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new TaskModel { Id = 1, Title = "Test Task", OwnerId = 1, StatusId = 1 });
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });

        // Act
        var act = async () => await CommentHelper.ValidateCommentAsync(comment, _mockTaskRepository.Object, _mockUserRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateCommentAsync_WithNullContent_ThrowsValidationException()
    {
        // Arrange
        var comment = new Comment
        {
            TaskId = 1,
            UserId = 1,
            Content = null!
        };

        // Act
        var act = async () => await CommentHelper.ValidateCommentAsync(comment, _mockTaskRepository.Object, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Comment content is required");
    }

    [Fact]
    public async Task ValidateCommentAsync_WithEmptyContent_ThrowsValidationException()
    {
        // Arrange
        var comment = new Comment
        {
            TaskId = 1,
            UserId = 1,
            Content = ""
        };

        // Act
        var act = async () => await CommentHelper.ValidateCommentAsync(comment, _mockTaskRepository.Object, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Comment content is required");
    }

    [Fact]
    public async Task ValidateCommentAsync_WithWhitespaceContent_ThrowsValidationException()
    {
        // Arrange
        var comment = new Comment
        {
            TaskId = 1,
            UserId = 1,
            Content = "   "
        };

        // Act
        var act = async () => await CommentHelper.ValidateCommentAsync(comment, _mockTaskRepository.Object, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Comment content is required");
    }

    [Fact]
    public async Task ValidateCommentAsync_WithNonExistentTask_ThrowsNotFoundException()
    {
        // Arrange
        var comment = new Comment
        {
            TaskId = 999,
            UserId = 1,
            Content = "Test comment"
        };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((TaskModel?)null);

        // Act
        var act = async () => await CommentHelper.ValidateCommentAsync(comment, _mockTaskRepository.Object, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Task not found");
    }

    [Fact]
    public async Task ValidateCommentAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        var comment = new Comment
        {
            TaskId = 1,
            UserId = 999,
            Content = "Test comment"
        };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new TaskModel { Id = 1, Title = "Test Task", OwnerId = 1, StatusId = 1 });
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await CommentHelper.ValidateCommentAsync(comment, _mockTaskRepository.Object, _mockUserRepository.Object);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }
}
