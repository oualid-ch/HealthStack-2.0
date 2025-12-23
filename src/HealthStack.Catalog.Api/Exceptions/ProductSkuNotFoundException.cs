namespace HealthStack.Catalog.Api.Exceptions;
public class ProductSkuNotFoundException(string sku) : Exception($"Product not found for SKU {sku}")
{
    public string Sku { get; } = sku;
}