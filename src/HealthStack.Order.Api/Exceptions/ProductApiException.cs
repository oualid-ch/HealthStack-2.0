namespace HealthStack.Order.Api.Exceptions;
public class ProductApiException(Guid productId) : Exception($"Unknown exception occurred while fetching product with Id {productId}")
{
    public Guid ProductId { get; } = productId;
}