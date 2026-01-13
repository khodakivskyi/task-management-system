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

    [Theory]
    [InlineData(1, 1, "project", "update")]
    [InlineData(5, 5, "task", "delete")]
    [InlineData(100, 100, "comment", "edit")]
    public void EnsureOwnership_WithVariousMatchingIds_DoesNotThrow(int ownerId, int requestingUserId, string entityName, string operation)
    {
        // Act
        var act = () => AuthorizationHelper.EnsureOwnership(ownerId, requestingUserId, entityName, operation);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(5, 10)]
    [InlineData(100, 1)]
    public void EnsureOwnership_WithDifferentIds_AlwaysThrows(int ownerId, int requestingUserId)
    {
        // Act
        var act = () => AuthorizationHelper.EnsureOwnership(ownerId, requestingUserId, "entity");

        // Assert
        act.Should().Throw<UnauthorizedException>();
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(999, 999)]
    public void EnsureCommentOwnership_WithVariousMatchingIds_DoesNotThrow(int authorId, int requestingUserId)
    {
        // Act
        var act = () => AuthorizationHelper.EnsureCommentOwnership(authorId, requestingUserId, "update");

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(10, 1)]
    [InlineData(999, 1)]
    public void EnsureCommentOwnership_WithDifferentIds_AlwaysThrows(int authorId, int requestingUserId)
    {
        // Act
        var act = () => AuthorizationHelper.EnsureCommentOwnership(authorId, requestingUserId);

        // Assert
        act.Should().Throw<UnauthorizedException>();
    }

    [Fact]
    public void EnsureOwnership_WithZeroIds_HandlesCorrectly()
    {
        // Arrange & Act
        var act1 = () => AuthorizationHelper.EnsureOwnership(0, 0, "project");
        var act2 = () => AuthorizationHelper.EnsureOwnership(0, 1, "project");

        // Assert
        act1.Should().NotThrow(); // Both are 0, so they match
        act2.Should().Throw<UnauthorizedException>();
    }

    [Fact]
    public void EnsureCommentOwnership_WithZeroIds_HandlesCorrectly()
    {
        // Arrange & Act
        var act1 = () => AuthorizationHelper.EnsureCommentOwnership(0, 0);
        var act2 = () => AuthorizationHelper.EnsureCommentOwnership(0, 1);

        // Assert
        act1.Should().NotThrow(); // Both are 0, so they match
        act2.Should().Throw<UnauthorizedException>();
    }

    [Fact]
    public void EnsureOwnership_WithNegativeIds_HandlesCorrectly()
    {
        // Arrange & Act
        var act1 = () => AuthorizationHelper.EnsureOwnership(-1, -1, "task");
        var act2 = () => AuthorizationHelper.EnsureOwnership(-1, 1, "task");

        // Assert
        act1.Should().NotThrow(); // Both are -1, so they match
        act2.Should().Throw<UnauthorizedException>();
    }

    [Theory]
    [InlineData("add")]
    [InlineData("remove")]
    [InlineData("modify")]
    [InlineData("access")]
    public void EnsureOwnership_WithDifferentOperations_IncludesOperationInMessage(string operation)
    {
        // Arrange
        var ownerId = 1;
        var requestingUserId = 2;

        // Act
        var act = () => AuthorizationHelper.EnsureOwnership(ownerId, requestingUserId, "resource", operation);

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage($"Only the resource owner can {operation} this resource");
    }

    [Theory]
    [InlineData("project")]
    [InlineData("task")]
    [InlineData("document")]
    public void EnsureOwnership_WithDifferentEntityNames_IncludesNameInMessage(string entityName)
    {
        // Arrange
        var ownerId = 1;
        var requestingUserId = 2;

        // Act
        var act = () => AuthorizationHelper.EnsureOwnership(ownerId, requestingUserId, entityName, "delete");

        // Assert
        act.Should().Throw<UnauthorizedException>()
            .WithMessage($"Only the {entityName} owner can delete this {entityName}");
    }
}
