using System.Security.Claims;
using backend.Models;
using backend.Models.DTOs;

namespace backend.Services.Interfaces;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal);
}
