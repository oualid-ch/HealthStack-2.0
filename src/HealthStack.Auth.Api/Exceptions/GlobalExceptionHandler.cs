using HealthStack.Auth.Api.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HealthStack.Auth.Api.Exceptions
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        readonly ILogger<GlobalExceptionHandler> _logger = logger;
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode;
            string title;
            string detail;

            switch (exception)
            {
                case UserNotFoundException ex:
                    statusCode = StatusCodes.Status404NotFound;
                    title = "Invalid credentials";
                    detail = "The credentials provided are invalid.";
                    _logger.LogWarning(ex, "User not found. Email: {Email}", LogUtils.MaskEmail(ex.Email));
                    break;
                
                case UserIdNotFoundException ex:
                    statusCode = StatusCodes.Status404NotFound;
                    title = "User not found";
                    detail = "No user exists with the specified ID.";
                    _logger.LogWarning(ex, "User not found. userId: {UserId}", LogUtils.MaskGuid(ex.UserId));
                    break;

                case InvalidPasswordException ex:
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Invalid credentials";
                    detail = "The credentials provided are invalid.";
                    _logger.LogWarning(ex, "Invalid password attempt. UserId: {UserId}", LogUtils.MaskGuid(ex.UserId));
                    break;

                case EmailAlreadyExistsException ex:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Email already exists";
                    detail = "The email provided is already registered.";
                    _logger.LogWarning(ex, "Email already exists. Email: {Email}", LogUtils.MaskEmail(ex.Email));
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    title = "Internal server error";
                    detail = "An unexpected error occurred.";
                    _logger.LogError(exception, "Unhandled exception at {Path}", httpContext.Request.Path);
                    break;
            }

            ProblemDetails problemDetails = new()
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}