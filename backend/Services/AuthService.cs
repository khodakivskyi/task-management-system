using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Configuration;
using backend.Exceptions;
using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Models.DTOs;
using backend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtOptions _jwtOptions;

    public AuthService(IUserRepository userRepository, JwtOptions jwtOptions)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        AuthHelper.ValidateRegistrationRequest(request);

        await AuthHelper.ValidateUserUniquenessAsync(request.Login, request.Email, _userRepository);

        AuthHelper.ValidatePassword(request.Password);

        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);

        var user = new User
        {
            Name = request.Name.Trim(),
            Surname = request.Surname?.Trim(),
            Email = request.Email.ToLower().Trim(),
            Login = request.Login.Trim(),
            PasswordHash = passwordHash,
            Salt = salt,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            EmailConfirmed = false
        };

        user.Id = await _userRepository.CreateAsync(user);

        string token = AuthHelper.GenerateJwtToken(user, _jwtOptions.Secret, _jwtOptions.ExpirationHours, _jwtOptions.Issuer, _jwtOptions.Audience);
        var expiresAt = DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours);

        return new AuthResponse(
            token,
            expiresAt,
            AuthHelper.MapToUserDto(user)
        );

    }
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        AuthHelper.ValidateLoginRequest(request);

        var user = await _userRepository.GetByLoginOrEmailAsync(request.LoginOrEmail);
        if (user == null)
            throw new UnauthorizedException("Invalid credentials");

        bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isValidPassword)
            throw new UnauthorizedException("Invalid credentials");

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        string token = AuthHelper.GenerateJwtToken(user, _jwtOptions.Secret, _jwtOptions.ExpirationHours, _jwtOptions.Issuer, _jwtOptions.Audience);
        var expiresAt = DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours);

        return new AuthResponse(
            token,
            expiresAt,
            AuthHelper.MapToUserDto(user)
        );
    }
    public async Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        var user = await AuthHelper.ExtractUserFromClaims(claimsPrincipal, _userRepository);
        if (user == null) return null;

        return AuthHelper.MapToUserDto(user);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
