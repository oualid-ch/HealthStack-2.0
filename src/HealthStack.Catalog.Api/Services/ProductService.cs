using Gridify;
using Gridify.EntityFramework;
using HealthStack.Catalog.Api.Data;
using HealthStack.Catalog.Api.Exceptions;
using HealthStack.Catalog.Api.Models;
using Microsoft.EntityFrameworkCore;
using HealthStack.Catalog.Api.Mapping;
using HealthStack.Catalog.Api.Auth;
using HealthStack.Catalog.Api.DTOs;

namespace HealthStack.Catalog.Api.Services;

public class ProductService(
    AppDbContext context,
    ILogger<ProductService> logger,
    ICurrentUser currentUser
) : IProductService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<ProductService> _logger = logger;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Product> GetProductByIdAsync(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .SingleOrDefaultAsync(p => p.Id == id && p.IsActive);
        
        return product ?? throw new ProductIdNotFoundException(id);
    }
    
    public async Task<Product> GetProductBySkuAsync(string sku)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .SingleOrDefaultAsync(p => p.Sku == sku && p.IsActive);

    return product ?? throw new ProductSkuNotFoundException(sku);
    }

    public async Task<Paging<Product>> GetProductsAsync(GridifyQuery query)
    {
        var mapper = new ProductGridifyMapper();

        return await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .GridifyAsync(query, mapper);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        if (await _context.Products.AnyAsync(p => p.Sku == product.Sku))
            throw new ProductSkuAlreadyExistsException(product.Sku);

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Product created. ProductId={ProductId}, AdminId={AdminId}",
            product.Id,
            _currentUser.UserId
        );
        
        return product;
    }

    public async Task<Product> UpdateProductAsync(Guid id, ProductUpdateDto product)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            throw new ProductIdNotFoundException(id);
        }

        _context.Entry(existingProduct).CurrentValues.SetValues(product);
        existingProduct.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Product updated. ProductId={ProductId}, AdminId={AdminId}",
            id,
            _currentUser.UserId
        );

        return existingProduct;
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            throw new ProductIdNotFoundException(id);
        }
        existingProduct.IsActive = false;
        existingProduct.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Product deleted. ProductId={ProductId}, AdminId={AdminId}",
            id,
            _currentUser.UserId
        );
    }

    public async Task RestoreProductAsync(Guid id)
    {
        var existingProduct = await _context.Products
            // .IgnoreQueryFilters() // important in case of global filters
            .SingleOrDefaultAsync(p => p.Id == id);

        if (existingProduct == null)
            throw new ProductIdNotFoundException(id);

        if (existingProduct.IsActive)
            return;

        existingProduct.IsActive = true;
        existingProduct.DeletedAt = null;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Product restored. ProductId={ProductId}, AdminId={AdminId}",
            id,
            _currentUser.UserId
        );
    }

}