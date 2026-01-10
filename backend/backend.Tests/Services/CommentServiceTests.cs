using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<IRepository<Comment>> _mockCommentRepository;
    private readonly Mock<IRepository<TaskModel>> _mockTaskRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _mockCommentRepository = new Mock<IRepository<Comment>>();
        _mockTaskRepository = new Mock<IRepository<TaskModel>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _service = new CommentService(_mockCommentRepository.Object, _mockTaskRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidComment_ReturnsCreatedComment()
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
        _mockCommentRepository.Setup(x => x.CreateAsync(It.IsAny<Comment>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(comment);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Content.Should().Be("Test comment");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockCommentRepository.Verify(x => x.CreateAsync(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentTask_ThrowsNotFoundException()
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
        var act = async () => await _service.CreateAsync(comment);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Task not found");
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentUser_ThrowsNotFoundException()
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
        var act = async () => await _service.CreateAsync(comment);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task CreateAsync_WithEmptyContent_ThrowsValidationException()
    {
        // Arrange
        var comment = new Comment
        {
            TaskId = 1,
            UserId = 1,
            Content = ""
        };

        _mockTaskRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new TaskModel { Id = 1, Title = "Test Task", OwnerId = 1, StatusId = 1 });
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });

        // Act
        var act = async () => await _service.CreateAsync(comment);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsComment()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 1,
            TaskId = 1,
            UserId = 1,
            Content = "Test comment",
            CreatedAt = DateTime.UtcNow
        };

        _mockCommentRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(comment);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Content.Should().Be("Test comment");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.GetByIdAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Comment id must be greater than 0");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockCommentRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Comment?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllComments()
    {
        // Arrange
        var comments = new List<Comment>
        {
            new() { Id = 1, TaskId = 1, UserId = 1, Content = "Comment 1", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, TaskId = 1, UserId = 2, Content = "Comment 2", CreatedAt = DateTime.UtcNow },
            new() { Id = 3, TaskId = 2, UserId = 1, Content = "Comment 3", CreatedAt = DateTime.UtcNow }
        };

        _mockCommentRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(comments);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Content == "Comment 1");
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesComment()
    {
        // Arrange
        var existingComment = new Comment
        {
            Id = 1,
            TaskId = 1,
            UserId = 1,
            Content = "Old content",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updatedComment = new Comment
        {
            Id = 1,
            TaskId = 2, // Try to change TaskId (should be prevented)
            UserId = 2, // Try to change UserId (should be prevented)
            Content = "New content",
            CreatedAt = DateTime.UtcNow // Try to change CreatedAt (should be prevented)
        };

        _mockCommentRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingComment);
        _mockTaskRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new TaskModel { Id = 1, Title = "Test Task", OwnerId = 1, StatusId = 1 });
        _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User { Id = 1, Login = "testuser", Email = "test@test.com", IsActive = true });
        _mockCommentRepository.Setup(x => x.UpdateAsync(It.IsAny<Comment>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAsync(updatedComment);

        // Assert
        result.Should().NotBeNull();
        result.Content.Should().Be("New content");
        result.TaskId.Should().Be(1); // Should be preserved
        result.UserId.Should().Be(1); // Should be preserved
        result.CreatedAt.Should().Be(existingComment.CreatedAt); // Should be preserved
        _mockCommentRepository.Verify(x => x.UpdateAsync(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 999,
            TaskId = 1,
            UserId = 1,
            Content = "Test"
        };

        _mockCommentRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Comment?)null);

        // Act
        var act = async () => await _service.UpdateAsync(comment);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Comment not found");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 0,
            TaskId = 1,
            UserId = 1,
            Content = "Test"
        };

        // Act
        var act = async () => await _service.UpdateAsync(comment);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesComment()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 1,
            TaskId = 1,
            UserId = 1,
            Content = "Test",
            CreatedAt = DateTime.UtcNow
        };

        _mockCommentRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(comment);
        _mockCommentRepository.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockCommentRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        _mockCommentRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Comment?)null);

        // Act
        var act = async () => await _service.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Comment not found");
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsBadRequestException()
    {
        // Act
        var act = async () => await _service.DeleteAsync(0);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryFails_ThrowsNotFoundException()
    {
        // Arrange
        var comment = new Comment
        {
            Id = 1,
            TaskId = 1,
            UserId = 1,
            Content = "Test",
            CreatedAt = DateTime.UtcNow
        };

        _mockCommentRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(comment);
        _mockCommentRepository.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Failed to delete comment");
    }
}
