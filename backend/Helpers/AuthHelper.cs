using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using backend.Exceptions;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Models.DTOs;

namespace backend.Helpers;

public class AuthHelper
{

    public static void SetConfiguration()
    {
    }

    public static async Task ValidateUserUniquenessAsync(string login, string email, IUserRepository userRepository)
    {
        var existingUserByLogin = await userRepository.GetByLoginAsync(login);
        if (existingUserByLogin != null)
        {
            throw new ConflictException($"User with login '{login}' already exists");
        }

        var existingUserByEmail = await userRepository.GetByEmailAsync(email);
        if (existingUserByEmail != null)
        {
            throw new ConflictException($"User with email '{email}' already exists");
        }
    }

    public static async Task<bool> IsLoginTakenAsync(string login, IUserRepository userRepository)
    {
        var user = await userRepository.GetByLoginAsync(login);
        return user != null;
    }

    public static async Task<bool> IsEmailTakenAsync(string email, IUserRepository userRepository)
    {
        var user = await userRepository.GetByEmailAsync(email);
        return user != null;
    }

    public static void ValidateRegistrationRequest(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Name is required");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");

        if (!IsValidEmail(request.Email))
            throw new ValidationException("Invalid email format");

        if (string.IsNullOrWhiteSpace(request.Login))
            throw new ValidationException("Login is required");

        if (request.Login.Length < 3)
            throw new ValidationException("Login must be at least 3 characters");

        if (request.Login.Length > 50)
            throw new ValidationException("Login must be at most 50 characters");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required");
    }

    public static void ValidatePassword(string password)
    {
        if (password.Length < 8)
            throw new ValidationException("Password must be at least 8 characters long");

        if (password.Length > 128)
            throw new ValidationException("Password must be at most 128 characters");

        if (!password.Any(char.IsUpper))
            throw new ValidationException("Password must contain at least one uppercase letter");

        if (!password.Any(char.IsDigit))
            throw new ValidationException("Password must contain at least one digit");
    }

    public static bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(email);
    }

    public static string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(GetJwtSecret());

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id. ToString()),
            new Claim(JwtRegisteredClaimNames. Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim("login", user.Login),
            new Claim(JwtRegisteredClaimNames. Jti, Guid.NewGuid().ToString()),
        };

        if (!string.IsNullOrEmpty(user.Surname))
        {
            claims.Add(new Claim("surname", user.Surname));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(GetTokenExpirationHours()),
            Issuer = GetJwtIssuer(),
            Audience = GetJwtAudience(),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static UserDto MapToUserDto(User user) => new(
        user.Id,
        user.Name,
        user.Surname,
        user.Email,
        user.Login,
        user.CreatedAt,
        user.LastLoginAt,
        user.EmailConfirmed
    );

    private static string GetJwtSecret()
    {
        var secret = _configuration["JWT_SECRET"] ??
                     Environment.GetEnvironmentVariable("JWT_SECRET");

        if (string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JWT_SECRET is not configured");

        if (secret.Length < 32)
            throw new InvalidOperationException("JWT_SECRET must be at least 32 characters");

        return secret;
    }

    public static string GetJwtIssuer() =>
        _configuration["JWT_ISSUER"] ??
        Environment.GetEnvironmentVariable("JWT_ISSUER") ??
        "TaskManagementSystem";

    public static string GetJwtAudience() =>
        _configuration["JWT_AUDIENCE"] ??
        Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
        "TaskManagementSystem";

    public static int GetTokenExpirationHours()
    {
        var hours = _configuration["JWT_EXPIRATION_HOURS"] ??
                    Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS") ??
                    "24";

        return int.Parse(hours);
    }
}
