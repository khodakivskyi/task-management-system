using backend.Exceptions;

namespace backend.Helpers;

/// <summary>
/// Common validation helper methods
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates that an ID is greater than 0
    /// </summary>
    public static void ValidateId(int id, string entityName)
    {
        if (id <= 0)
        {
            throw new BadRequestException($"{entityName} id must be greater than 0");
        }
    }
}
