using backend.Exceptions;
using FluentAssertions;

namespace backend.Tests.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Validation error";

        // Act
        var exception = new ValidationException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.Errors.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndErrors_ShouldSetBoth()
    {
        // Arrange
        var message = "Validation error";
        var errors = new Dictionary<string, string[]>
        {
            { "Field1", new[] { "Error1", "Error2" } },
            { "Field2", new[] { "Error3" } }
        };

        // Act
        var exception = new ValidationException(message, errors);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().ContainKey("Field1");
        exception.Errors.Should().ContainKey("Field2");
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        var message = "Validation error";
        var innerException = new Exception("Inner error");

        // Act
        var exception = new ValidationException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void Exception_ShouldBeOfTypeException()
    {
        // Arrange & Act
        var exception = new ValidationException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
