using backend.Exceptions;
using FluentAssertions;

namespace backend.Tests.Exceptions;

public class ForbiddenExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Forbidden access";

        // Act
        var exception = new ForbiddenException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Exception_ShouldBeOfTypeException()
    {
        // Arrange & Act
        var exception = new ForbiddenException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
