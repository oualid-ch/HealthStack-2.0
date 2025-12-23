namespace HealthStack.Catalog.Api.Exceptions;
public class ProductIdNotFoundException(Guid productId) : Exception($"Product not found for Id {productId}")
{
    public Guid ProductId { get; } = productId;
}