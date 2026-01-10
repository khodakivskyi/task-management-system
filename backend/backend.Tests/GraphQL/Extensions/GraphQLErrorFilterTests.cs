using backend.Exceptions;
using backend.GraphQL.Extensions;
using FluentAssertions;
using HotChocolate;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.Tests.GraphQL.Extensions;

public class GraphQLErrorFilterTests
{
    private readonly Mock<ILogger<GraphQLErrorFilter>> _mockLogger;
    private readonly GraphQLErrorFilter _filter;

    public GraphQLErrorFilterTests()
    {
        _mockLogger = new Mock<ILogger<GraphQLErrorFilter>>();
        _filter = new GraphQLErrorFilter(_mockLogger.Object);
    }

    [Fact]
    public void OnError_WithValidationException_ReturnsValidationErrorCode()
    {
        // Arrange
        var exception = new ValidationException("Validation failed");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Code.Should().Be("VALIDATION_ERROR");
        result.Message.Should().Be("Validation failed");
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void OnError_WithNotFoundException_ReturnsNotFoundCode()
    {
        // Arrange
        var exception = new NotFoundException("Resource not found");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Code.Should().Be("NOT_FOUND");
        result.Message.Should().Be("Resource not found");
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void OnError_WithUnauthorizedException_ReturnsUnauthorizedCode()
    {
        // Arrange
        var exception = new UnauthorizedException("Unauthorized access");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Code.Should().Be("UNAUTHORIZED");
        result.Message.Should().Be("Unauthorized access");
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void OnError_WithForbiddenException_ReturnsForbiddenCode()
    {
        // Arrange
        var exception = new ForbiddenException("Access forbidden");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Code.Should().Be("FORBIDDEN");
        result.Message.Should().Be("Access forbidden");
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void OnError_WithConflictException_ReturnsConflictCode()
    {
        // Arrange
        var exception = new ConflictException("Resource conflict");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Code.Should().Be("CONFLICT");
        result.Message.Should().Be("Resource conflict");
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void OnError_WithBadRequestException_ReturnsBadRequestCode()
    {
        // Arrange
        var exception = new BadRequestException("Bad request");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Code.Should().Be("BAD_REQUEST");
        result.Message.Should().Be("Bad request");
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void OnError_WithGenericException_ReturnsInternalErrorCode()
    {
        // Arrange
        var exception = new Exception("Internal error");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Code.Should().Be("INTERNAL_ERROR");
        result.Message.Should().Be("Internal error");
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void OnError_WithValidationExceptionWithErrors_IncludesErrorsInExtension()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Field1", new[] { "Error1", "Error2" } },
            { "Field2", new[] { "Error3" } }
        };
        var exception = new ValidationException("Validation failed", errors);
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Code.Should().Be("VALIDATION_ERROR");
        result.Extensions.Should().ContainKey("errors");
        result.Extensions!["errors"].Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void OnError_WithNullException_ReturnsOriginalError()
    {
        // Arrange
        var error = ErrorBuilder.New()
            .SetMessage("Error without exception")
            .Build();

        // Act
        var result = _filter.OnError(error);

        // Assert
        result.Should().BeSameAs(error);
    }

    [Fact]
    public void OnError_WithValidationException_LogsWarning()
    {
        // Arrange
        var exception = new ValidationException("Validation failed");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        _filter.OnError(error);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void OnError_WithNotFoundException_LogsWarning()
    {
        // Arrange
        var exception = new NotFoundException("Not found");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        _filter.OnError(error);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void OnError_WithGenericException_LogsError()
    {
        // Arrange
        var exception = new Exception("Internal error");
        var error = ErrorBuilder.New()
            .SetMessage("Original message")
            .SetException(exception)
            .Build();

        // Act
        _filter.OnError(error);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
