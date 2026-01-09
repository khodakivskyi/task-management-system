using backend.Models.DTOs;
using backend.Services.Interfaces;

namespace backend.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for authentication
/// </summary>
public class AuthMutation
{
    public async Task<AuthResponse> Register(
        RegisterRequest input,
        [Service] IAuthService authService)
    {
        return await authService.RegisterAsync(input);
    }

    public async Task<AuthResponse> Login(
    LoginRequest input,
    [Service] IAuthService authService)
    {
        return await authService.LoginAsync(input);
    }
}
