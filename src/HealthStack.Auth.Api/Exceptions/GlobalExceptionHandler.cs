using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HealthStack.Auth.Api.Exceptions
{
    // public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    public sealed class GlobalExceptionHandler() : IExceptionHandler
    {
        // readonly ILogger<GlobalExceptionHandler> _logger = logger;
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // TODO: Logging
            // _logger.LogError(exception, $"Error Message: {exception.Message}, Occurred at: {DateTime.UtcNow}");

            ProblemDetails problemDetails = new()
            {
                Title = exception.GetType().Name,
                Detail = exception.Message,
                Status = StatusCodes.Status500InternalServerError,
                Instance = httpContext.Request.Path
            };

            problemDetails.Status = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                EmailAlreadyExistsException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            };

            
            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}