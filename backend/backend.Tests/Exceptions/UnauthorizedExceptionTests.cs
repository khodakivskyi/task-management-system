using backend.Exceptions;
using FluentAssertions;

namespace backend.Tests.Exceptions;

public class UnauthorizedExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Unauthorized error";

        // Act
        var exception = new UnauthorizedException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Exception_ShouldBeOfTypeException()
    {
        // Arrange & Act
        var exception = new UnauthorizedException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
