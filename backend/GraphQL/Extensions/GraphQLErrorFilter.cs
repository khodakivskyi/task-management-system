using backend.Exceptions;
using HotChocolate;

namespace backend.GraphQL.Extensions;

/// <summary>
/// Custom GraphQLErrorFilter for GraphQL to map exceptions to error codes and log errors. 
/// </summary>
public class GraphQLErrorFilter : IErrorFilter
{
    private readonly ILogger<GraphQLErrorFilter> _logger;

    public GraphQLErrorFilter(ILogger<GraphQLErrorFilter> logger)
    {
        _logger = logger;
    }

    public IError OnError(IError error)
    {
        var exception = error.Exception;

        if (exception is null)
            return error;

        // Error logging
        if (exception is not ValidationException and not NotFoundException)
        {
            _logger.LogError(exception, "GraphQL error: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning(
                "GraphQL handled error: {Type} - {Message}",
                exception.GetType().Name,
                exception.Message
            );
        }

        // Map exceptions to error codes
        var code = exception switch
        {
            UnauthorizedException => "UNAUTHORIZED",
            ForbiddenException => "FORBIDDEN",
            ConflictException => "CONFLICT",
            NotFoundException => "NOT_FOUND",
            ValidationException => "VALIDATION_ERROR",
            BadRequestException => "BAD_REQUEST",
            _ => "INTERNAL_ERROR"
        };

        var builder = error
            .WithMessage(exception.Message) 
            .WithCode(code)
            .WithException(null);

        // Add validation errors if present
        if (exception is ValidationException ve && ve.Errors is not null)
        {
            builder = builder.SetExtension("errors", ve.Errors);
        }

        return builder;
    }
}