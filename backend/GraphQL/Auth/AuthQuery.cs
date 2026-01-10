using System.Security.Claims;
using backend.Models.DTOs;
using backend.Services.Interfaces;
using HotChocolate.Authorization;

namespace backend.GraphQL.Queries;

/// <summary>
/// GraphQL queries for authentication
/// </summary>
public class AuthQuery
{
    [Authorize]
    public async Task<UserDto?> Me(
        ClaimsPrincipal claimsPrincipal,
        [Service] IAuthService authService
        )
    {
        return await authService.GetCurrentUserAsync(claimsPrincipal);
    }
}
