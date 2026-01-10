using backend.Exceptions;
using backend.Helpers;
using FluentAssertions;

namespace backend.Tests.Helpers;

public class ValidationHelperTests
{
    [Fact]
    public void ValidateId_WithValidId_ShouldNotThrow()
    {
        // Arrange
        var id = 1;
        var entityName = "TestEntity";

        // Act
        var act = () => ValidationHelper.ValidateId(id, entityName);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ValidateId_WithInvalidId_ShouldThrowBadRequestException(int invalidId)
    {
        // Arrange
        var entityName = "TestEntity";

        // Act
        var act = () => ValidationHelper.ValidateId(invalidId, entityName);

        // Assert
        act.Should().Throw<BadRequestException>()
            .WithMessage($"{entityName} id must be greater than 0");
    }

    [Fact]
    public void ValidateId_WithPositiveId_ShouldNotThrow()
    {
        // Arrange
        var id = 999;
        var entityName = "User";

        // Act
        var act = () => ValidationHelper.ValidateId(id, entityName);

        // Assert
        act.Should().NotThrow();
    }
}
