using backend.Exceptions;
using FluentAssertions;

namespace backend.Tests.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Resource not found";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Exception_ShouldBeOfTypeException()
    {
        // Arrange & Act
        var exception = new NotFoundException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
