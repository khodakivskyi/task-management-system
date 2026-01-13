using backend.Exceptions;

namespace backend.Helpers;

/// <summary>
/// Helper for authorization checks
/// </summary>
public static class AuthorizationHelper
{
    /// <summary>
    /// Ensures that the requesting user is the owner of the entity
    /// </summary>
    public static void EnsureOwnership(int expectedOwnerId, int requestingUserId, string entityName, string operation = "perform this operation on")
    {
        if (expectedOwnerId != requestingUserId)
        {
            throw new UnauthorizedException($"Only the {entityName} owner can {operation} this {entityName}");
        }
    }

    /// <summary>
    /// Ensures that the requesting user is the author of the comment
    /// </summary>
    public static void EnsureCommentOwnership(int expectedUserId, int requestingUserId, string operation = "perform this operation on")
    {
        if (expectedUserId != requestingUserId)
        {
            throw new UnauthorizedException($"Only the comment author can {operation} this comment");
        }
    }
}
