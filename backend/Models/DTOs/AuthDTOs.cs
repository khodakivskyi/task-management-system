namespace backend.Models.DTOs;

/// <summary>
/// Request for user registration
/// </summary>
public record RegisterRequest(
    string Name,
    string? Surname,
    string Email,
    string Login,
    string Password
);

/// <summary>
/// Request for user login
/// </summary>
public record LoginRequest(
    string LoginOrEmail,
    string Password
);

/// <summary>
/// Response after successful authentication
/// </summary>
public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    UserDto User
);

/// <summary>
/// User DTO
/// </summary>
public record UserDto(
    int Id,
    string Name,
    string? Surname,
    string Email,
    string Login,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    bool EmailConfirmed
);
