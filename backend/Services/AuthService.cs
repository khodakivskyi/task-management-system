using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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

        string token = AuthHelper.GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(AuthHelper.GetTokenExpirationHours());

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

        if (isValidPassword)
            throw new UnauthorizedException("Invalid login credentials");

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        string token = AuthHelper.GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(AuthHelper.GetTokenExpirationHours());

        return new AuthResponse(
            token,
            expiresAt,
            AuthHelper.MapToUserDto(user)
        );
    }
    public async Task<User?> GetUserFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return await _userRepository.GetByIdAsync(userId);
        }
        catch
        {
            return null;
        }
    }
    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(AuthHelper.GetJwtSecret());

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = AuthHelper.GetJwtIssuer(),
                ValidateAudience = true,
                ValidAudience = AuthHelper.GetJwtAudience(),
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
