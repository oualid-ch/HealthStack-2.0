namespace HealthStack.Order.Api.Exceptions;
public class ProductNotFoundException(Guid productId) : Exception($"Product not found for Id {productId}")
{
    public Guid ProductId { get; } = productId;
}