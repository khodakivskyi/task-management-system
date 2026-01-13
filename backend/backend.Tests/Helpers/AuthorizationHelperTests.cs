using backend.Exceptions;
using backend.Helpers;
using FluentAssertions;

namespace backend.Tests.Helpers;

public class AuthorizationHelperTests
{
    [Fact]
    public void EnsureOwnership_WithMatchingIds_DoesNotThrow()
    {
        // Arrange
        var ownerId = 1;
        var requestingUserId = 1;

        // Act
        var act = () => AuthorizationHelper.EnsureOwnership(ownerId, requestingUserId, "project", "update");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureOwnership_WithDifferentIds_ThrowsUnauthorizedException()
    {
        // Arrange
        var ownerId = 1;
        var requestingUserId = 2;

        // Act
        var act = () => AuthorizationHelper.EnsureOwnership(ownerId, requestingUserId, "project", "update");

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Only the project owner can update this project");
    }

    [Fact]
    public void EnsureOwnership_WithDeleteOperation_ThrowsCorrectMessage()
    {
        // Arrange
        var ownerId = 1;
        var requestingUserId = 2;

        // Act
        var act = () => AuthorizationHelper.EnsureOwnership(ownerId, requestingUserId, "task", "delete");

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Only the task owner can delete this task");
    }

    [Fact]
    public void EnsureOwnership_WithDefaultOperation_ThrowsCorrectMessage()
    {
        // Arrange
        var ownerId = 1;
        var requestingUserId = 2;

        // Act
        var act = () => AuthorizationHelper.EnsureOwnership(ownerId, requestingUserId, "project");

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Only the project owner can perform this operation on this project");
    }

    [Fact]
    public void EnsureCommentOwnership_WithMatchingIds_DoesNotThrow()
    {
        // Arrange
        var authorId = 1;
        var requestingUserId = 1;

        // Act
        var act = () => AuthorizationHelper.EnsureCommentOwnership(authorId, requestingUserId, "update");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCommentOwnership_WithDifferentIds_ThrowsUnauthorizedException()
    {
        // Arrange
        var authorId = 1;
        var requestingUserId = 2;

        // Act
        var act = () => AuthorizationHelper.EnsureCommentOwnership(authorId, requestingUserId, "update");

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Only the comment author can update this comment");
    }

    [Fact]
    public void EnsureCommentOwnership_WithDeleteOperation_ThrowsCorrectMessage()
    {
        // Arrange
        var authorId = 1;
        var requestingUserId = 2;

        // Act
        var act = () => AuthorizationHelper.EnsureCommentOwnership(authorId, requestingUserId, "delete");

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Only the comment author can delete this comment");
    }

    [Fact]
    public void EnsureCommentOwnership_WithDefaultOperation_ThrowsCorrectMessage()
    {
        // Arrange
        var authorId = 1;
        var requestingUserId = 2;

        // Act
        var act = () => AuthorizationHelper.EnsureCommentOwnership(authorId, requestingUserId);

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage("Only the comment author can perform this operation on this comment");
    }
}
