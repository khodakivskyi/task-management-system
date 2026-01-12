using System.Security.Claims;
using backend.Configuration;
using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Models.DTOs;
using backend.Services;
using FluentAssertions;
using Moq;

namespace backend.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();

        // Set environment variables for JwtOptions
        Environment.SetEnvironmentVariable("JWT_SECRET", "very-long-secret-key-for-jwt-token-generation-minimum-32-characters");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "TestIssuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "TestAudience");
        Environment.SetEnvironmentVariable("JWT_EXPIRATION_HOURS", "24");

        _jwtOptions = JwtOptions.LoadFromEnvironment();
        _authService = new AuthService(_mockUserRepository.Object, _jwtOptions);
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "john@example.com", "johndoe", "Password123");
        _mockUserRepository.Setup(r => r.CheckUserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, false));
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(1);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Name.Should().Be("John");
        result.User.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task RegisterAsync_WithTakenLogin_ShouldThrowConflictException()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "john@example.com", "johndoe", "Password123");
        _mockUserRepository.Setup(r => r.CheckUserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, false));

        // Act
        var act = async () => await _authService.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Unable to create account with provided data");
    }

    [Fact]
    public async Task RegisterAsync_WithTakenEmail_ShouldThrowConflictException()
    {
        // Arrange
        var request = new RegisterRequest("John", "Doe", "john@example.com", "johndoe", "Password123");
        _mockUserRepository.Setup(r => r.CheckUserExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, true));

        // Act
        var act = async () => await _authService.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Unable to create account with provided data");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new LoginRequest("johndoe", "Password123");
        var user = new User
        {
            Id = 1,
            Name = "John",
            Surname = "Doe",
            Email = "john@example.com",
            Login = "johndoe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _mockUserRepository.Setup(r => r.GetByLoginOrEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Login.Should().Be("johndoe");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidLogin_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest("johndoe", "Password123");
        _mockUserRepository.Setup(r => r.GetByLoginOrEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest("johndoe", "WrongPassword");
        var user = new User
        {
            Id = 1,
            Name = "John",
            Email = "john@example.com",
            Login = "johndoe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _mockUserRepository.Setup(r => r.GetByLoginOrEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithValidUserId_ShouldReturnUserDto()
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
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _mockUserRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.GetCurrentUserAsync(claimsPrincipal);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("John");
        result.Login.Should().Be("johndoe");
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "John",
            Email = "john@example.com",
            Login = "johndoe",
            PasswordHash = "hash",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _mockUserRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.GetCurrentUserAsync(claimsPrincipal);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidateTokenAsync_WithValidToken_ShouldReturnTrue()
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
        var token = AuthHelper.GenerateJwtToken(user, _jwtOptions.Secret, _jwtOptions.ExpirationHours, _jwtOptions.Issuer, _jwtOptions.Audience);

        // Act
        var result = await _authService.ValidateTokenAsync(token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateTokenAsync_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var result = await _authService.ValidateTokenAsync(invalidToken);

        // Assert
        result.Should().BeFalse();
    }
}
