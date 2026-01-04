using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HealthStack.Order.Api.Exceptions;

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
            case OrderIdNotFoundException ex:
                statusCode = StatusCodes.Status404NotFound;
                title = "Order not found";
                detail = "No order exists with the specified ID.";
                _logger.LogWarning(ex, "Order not found. orderId: {OrderId}", ex.OrderId);
                break;

            case ProductNotFoundException ex:
                statusCode = StatusCodes.Status404NotFound;
                title = "Product not found";
                detail = "No product exists with the specified Id.";
                _logger.LogWarning(ex, "Product not found. Id: {Id}", ex.ProductId);
                break;

            case ProductApiException ex:
                statusCode = StatusCodes.Status502BadGateway;
                title = "Product API error";
                detail = "An error occurred while communicating with the Product API.";
                _logger.LogError(ex, "Product API error. Id: {Id}", ex.ProductId);
                break;

            // case ProductSkuNotFoundException ex:
            //     statusCode = StatusCodes.Status404NotFound;
            //     title = "Product not found";
            //     detail = "No product exists with the specified SKU.";
            //     _logger.LogWarning(ex, "Product not found. sku: {Sku}", ex.Sku);
            //     break;

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