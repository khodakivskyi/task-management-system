using backend.Exceptions;
using GraphQL;
using GraphQL.Execution;

namespace backend.GraphQL;

/// <summary>
/// Custom ErrorInfoProvider for GraphQL to map exceptions to error codes and log errors.
/// </summary>
public class CustomErrorInfoProvider : ErrorInfoProvider
{
    private readonly ILogger<CustomErrorInfoProvider> _logger;

    public CustomErrorInfoProvider(ILogger<CustomErrorInfoProvider> logger)
    {
        _logger = logger;
    }

    public override ErrorInfo GetInfo(ExecutionError executionError)
    {
        var info = base.GetInfo(executionError);
        var original = executionError.InnerException ?? executionError;

        // Error logging
        if (original is not ValidationException and not NotFoundException)
        {
            _logger.LogError(original, "GraphQL error occurred:  {Message}", original.Message);
        }
        else
        {
            _logger.LogWarning("GraphQL handled error: {ErrorType} - {Message}",
                original.GetType().Name, original.Message);
        }

        // Map exceptions to error codes
        info.Extensions!["code"] = original switch
        {
            UnauthorizedException => "UNAUTHORIZED",
            ForbiddenException => "FORBIDDEN",
            ConflictException => "CONFLICT",
            NotFoundException => "NOT_FOUND",
            ValidationException => "VALIDATION_ERROR",
            BadRequestException => "BAD_REQUEST",
            _ => "INTERNAL_ERROR"
        };

        if (original is ValidationException validationEx && validationEx.Errors != null)
        {
            info.Extensions["errors"] = validationEx.Errors;
        }

        return new ErrorInfo
        {
            Message = info.Message,
            Extensions = info.Extensions
        };
    }
}
