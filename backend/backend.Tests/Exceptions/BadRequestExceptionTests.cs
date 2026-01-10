using backend.Exceptions;
using FluentAssertions;

namespace backend.Tests.Exceptions;

public class BadRequestExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Bad request error";

        // Act
        var exception = new BadRequestException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        var message = "Bad request error";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new BadRequestException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void Exception_ShouldBeOfTypeException()
    {
        // Arrange & Act
        var exception = new BadRequestException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
