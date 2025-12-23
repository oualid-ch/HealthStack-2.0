using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HealthStack.Catalog.Api.Exceptions;

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
            case ProductIdNotFoundException ex:
                statusCode = StatusCodes.Status404NotFound;
                title = "Product not found";
                detail = "No product exists with the specified ID.";
                _logger.LogWarning(ex, "Product not found. productId: {ProductId}", ex.ProductId);
                break;

            case ProductSkuNotFoundException ex:
                statusCode = StatusCodes.Status404NotFound;
                title = "Product not found";
                detail = "No product exists with the specified SKU.";
                _logger.LogWarning(ex, "Product not found. sku: {Sku}", ex.Sku);
                break;
            
            case CategoryIdNotFoundException ex:
                statusCode = StatusCodes.Status404NotFound;
                title = "Category not found";
                detail = "No category exists with the specified ID.";
                _logger.LogWarning(ex, "Category not found. categoryId: {CategoryId}", ex.CategoryId);
                break;

            case ProductSkuAlreadyExistsException ex:
                statusCode = StatusCodes.Status409Conflict;
                title = "Product SKU already exists";
                detail = "The product SKU provided is already registered.";
                _logger.LogWarning(ex, "Product SKU already exists. SKU: {SKU}", ex.Sku);
                break;

            case CategoryNameAlreadyExistsException ex:
                statusCode = StatusCodes.Status409Conflict;
                title = "Category name already exists";
                detail = "The category name provided is already registered.";
                _logger.LogWarning(ex, "Category name already exists. Name: {Name}", ex.Name);
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