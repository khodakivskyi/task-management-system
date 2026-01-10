using backend.Exceptions;
using FluentAssertions;

namespace backend.Tests.Exceptions;

public class ConflictExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Resource conflict";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Exception_ShouldBeOfTypeException()
    {
        // Arrange & Act
        var exception = new ConflictException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
