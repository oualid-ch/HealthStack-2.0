using Gridify;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Models;

namespace HealthStack.Catalog.Api.Services;
public interface IProductService
{
    public Task<Product> GetProductByIdAsync(Guid id);
    public Task<Product> GetProductBySkuAsync(string sku);
    public Task<Paging<Product>> GetProductsAsync(GridifyQuery query);
    public Task<Product> AddProductAsync(Product product);
    public Task<Product> UpdateProductAsync(Guid id, ProductUpdateDto product);
    public Task DeleteProductAsync(Guid id);
    public Task RestoreProductAsync(Guid id);
}