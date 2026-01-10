using System.Security.Claims;
using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Models.DTOs;
using FluentAssertions;
using Moq;

namespace backend.Tests.Helpers;

public class AuthHelperTests
{
    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.uk", true)]
    [InlineData("invalid.email", false)]
    [InlineData("@example.com", false)]
    [InlineData("test@", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void IsValidEmail_ShouldValidateEmailCorrectly(string email, bool expected)
    {
        // Act
        var result = AuthHelper.IsValidEmail(email);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ValidateRegistrationRequest_WithValidRequest_ShouldNotThrow()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "john.doe@example.com", "johndoe", "Password1");

        // Act
        var act = () => AuthHelper.ValidateRegistrationRequest(request);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateRegistrationRequest_WithEmptyName_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest("", "Doe", "john.doe@example.com", "johndoe", "Password1");

        // Act
        var act = () => AuthHelper.ValidateRegistrationRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Name is required");
    }

    [Fact]
    public void ValidateRegistrationRequest_WithEmptyEmail_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "", "johndoe", "Password1");

        // Act
        var act = () => AuthHelper.ValidateRegistrationRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Email is required");
    }

    [Fact]
    public void ValidateRegistrationRequest_WithInvalidEmail_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "invalid-email", "johndoe", "Password1");

        // Act
        var act = () => AuthHelper.ValidateRegistrationRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Invalid email format");
    }

    [Fact]
    public void ValidateRegistrationRequest_WithEmptyLogin_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "john.doe@example.com", "", "Password1");

        // Act
        var act = () => AuthHelper.ValidateRegistrationRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Login is required");
    }

    [Fact]
    public void ValidateRegistrationRequest_WithShortLogin_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "john.doe@example.com", "ab", "Password1");

        // Act
        var act = () => AuthHelper.ValidateRegistrationRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Login must be at least 3 characters");
    }

    [Fact]
    public void ValidateRegistrationRequest_WithLongLogin_ShouldThrowValidationException()
    {
        // Arrange
        var longLogin = new string('a', 51);
        var request = new RegisterRequest("John", "Doe", "john.doe@example.com", longLogin, "Password1");

        // Act
        var act = () => AuthHelper.ValidateRegistrationRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Login must be at most 50 characters");
    }

    [Fact]
    public void ValidateRegistrationRequest_WithEmptyPassword_ShouldThrowValidationException()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "john.doe@example.com", "johndoe", "");

        // Act
        var act = () => AuthHelper.ValidateRegistrationRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Password is required");
    }

    [Fact]
    public void ValidateLoginRequest_WithValidRequest_ShouldNotThrow()
    {
        // Arrange
        var request = new LoginRequest("johndoe", "Password1");

        // Act
        var act = () => AuthHelper.ValidateLoginRequest(request);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateLoginRequest_WithEmptyLoginOrEmail_ShouldThrowValidationException()
    {
        // Arrange
        var request = new LoginRequest("", "Password1");

        // Act
        var act = () => AuthHelper.ValidateLoginRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Login or Email is required");
    }

    [Fact]
    public void ValidateLoginRequest_WithEmptyPassword_ShouldThrowValidationException()
    {
        // Arrange
        var request = new LoginRequest("johndoe", "");

        // Act
        var act = () => AuthHelper.ValidateLoginRequest(request);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Password is required");
    }

    [Fact]
    public void ValidatePassword_WithValidPassword_ShouldNotThrow()
    {
        // Arrange
        var password = "Password1";

        // Act
        var act = () => AuthHelper.ValidatePassword(password);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidatePassword_WithShortPassword_ShouldThrowValidationException()
    {
        // Arrange
        var password = "Pass1";

        // Act
        var act = () => AuthHelper.ValidatePassword(password);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Password must be at least 8 characters long");
    }

    [Fact]
    public void ValidatePassword_WithLongPassword_ShouldThrowValidationException()
    {
        // Arrange
        var password = new string('a', 129) + "A1";

        // Act
        var act = () => AuthHelper.ValidatePassword(password);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Password must be at most 128 characters");
    }

    [Fact]
    public void ValidatePassword_WithNoUppercase_ShouldThrowValidationException()
    {
        // Arrange
        var password = "password1";

        // Act
        var act = () => AuthHelper.ValidatePassword(password);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Password must contain at least one uppercase letter");
    }

    [Fact]
    public void ValidatePassword_WithNoDigit_ShouldThrowValidationException()
    {
        // Arrange
        var password = "Password";

        // Act
        var act = () => AuthHelper.ValidatePassword(password);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Password must contain at least one digit");
    }

    [Fact]
    public async Task ValidateUserUniquenessAsync_WithAvailableLoginAndEmail_ShouldNotThrow()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        mockRepository.Setup(r => r.CheckUserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, false));

        // Act
        var act = async () => await AuthHelper.ValidateUserUniquenessAsync("login", "email@test.com", mockRepository.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateUserUniquenessAsync_WithTakenLogin_ShouldThrowConflictException()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        mockRepository.Setup(r => r.CheckUserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, false));

        // Act
        var act = async () => await AuthHelper.ValidateUserUniquenessAsync("login", "email@test.com", mockRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Unable to create account with provided data");
    }

    [Fact]
    public async Task ValidateUserUniquenessAsync_WithTakenEmail_ShouldThrowConflictException()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        mockRepository.Setup(r => r.CheckUserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, true));

        // Act
        var act = async () => await AuthHelper.ValidateUserUniquenessAsync("login", "email@test.com", mockRepository.Object);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Unable to create account with provided data");
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Email = "john@example.com",
            Login = "johndoe",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        var secret = "very-long-secret-key-for-jwt-token-generation-min-32-chars";
        var expirationHours = 24;
        var issuer = "TestIssuer";
        var audience = "TestAudience";

        // Act
        var token = AuthHelper.GenerateJwtToken(user, secret, expirationHours, issuer, audience);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts
    }

    [Fact]
    public void MapToUserDto_ShouldMapUserCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Email = "john@example.com",
            Login = "johndoe",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            EmailConfirmed = true,
            IsActive = true
        };

        // Act
        var dto = AuthHelper.MapToUserDto(user);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(user.Id);
        dto.Name.Should().Be(user.Name);
        dto.Surname.Should().Be(user.Surname);
        dto.Email.Should().Be(user.Email);
        dto.Login.Should().Be(user.Login);
        dto.CreatedAt.Should().Be(user.CreatedAt);
        dto.LastLoginAt.Should().Be(user.LastLoginAt);
        dto.EmailConfirmed.Should().Be(user.EmailConfirmed);
    }

    [Fact]
    public void ExtractUserIdFromClaims_WithValidClaim_ShouldReturnUserId()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123")
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var userId = AuthHelper.ExtractUserIdFromClaims(claimsPrincipal);

        // Assert
        userId.Should().Be(123);
    }

    [Fact]
    public void ExtractUserIdFromClaims_WithSubClaim_ShouldReturnUserId()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("sub", "456")
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var userId = AuthHelper.ExtractUserIdFromClaims(claimsPrincipal);

        // Assert
        userId.Should().Be(456);
    }

    [Fact]
    public void ExtractUserIdFromClaims_WithNoClaim_ShouldReturnNull()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var userId = AuthHelper.ExtractUserIdFromClaims(claimsPrincipal);

        // Assert
        userId.Should().BeNull();
    }

    [Fact]
    public void ExtractUserIdFromClaims_WithInvalidClaim_ShouldReturnNull()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "not-a-number")
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var userId = AuthHelper.ExtractUserIdFromClaims(claimsPrincipal);

        // Assert
        userId.Should().BeNull();
    }
}
