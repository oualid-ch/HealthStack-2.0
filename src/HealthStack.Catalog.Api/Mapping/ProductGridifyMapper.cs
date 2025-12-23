using Gridify;
using HealthStack.Catalog.Api.Models;

namespace HealthStack.Catalog.Api.Mapping;

public class ProductGridifyMapper : GridifyMapper<Product>
{
    public ProductGridifyMapper()
    {
        AddMap("sku", p => p.Sku);
        AddMap("name", p => p.Name);
        AddMap("brand", p => p.Brand);
        AddMap("price", p => p.Price);
        AddMap("createdAt", p => p.CreatedAt);
        AddMap("categoryId", p => p.CategoryId);
    }
}