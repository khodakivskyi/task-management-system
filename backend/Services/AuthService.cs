using backend.Helpers;
using backend.Infrastructure.Repositories.Interfaces;
using backend.Models;
using backend.Models.DTOs;
using backend.Services.Interfaces;

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
        throw new NotImplementedException();
    }
    public async Task<User?> GetUserFromTokenAsync(string token)
    {
        throw new NotImplementedException();
    }
    public async Task<bool> ValidateTokenAsync(string token)
    {
        throw new NotImplementedException();
    }
}
