namespace HealthStack.Catalog.Api.Exceptions;
public class ProductSkuAlreadyExistsException(string sku) : Exception($"Product SKU '{sku}' already exists")
{
    public string Sku = sku;
}
