using System.Security.Claims;
using backend.Models.DTOs;
using backend.Services.Interfaces;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL queries for authentication
/// </summary>
public class AuthQuery
{
    public async Task<UserDto?> Me(
        ClaimsPrincipal claimsPrincipal,
        [Service] IAuthService authService
        )
    {
        return await authService.GetCurrentUserAsync(claimsPrincipal);
    }

}
