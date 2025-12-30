using Gridify;
using HealthStack.Catalog.Api.Auth;
using HealthStack.Catalog.Api.Data;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Exceptions;
using HealthStack.Catalog.Api.Models;
using HealthStack.Catalog.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthStack.Catalog.UnitTests;

public class ProductServiceUnitTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly ProductService _sut;

    public ProductServiceUnitTests()
    {
        // InMemory DB
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Mocks
        _loggerMock = new Mock<ILogger<ProductService>>();
        _currentUserMock = new Mock<ICurrentUser>();

        // SUT
        _sut = new ProductService(_context, _loggerMock.Object, _currentUserMock.Object);
    }

    private async Task<Product> SeedActiveProduct()
    {
        Guid CategoryId = Guid.NewGuid();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Sku = "TESTSKU",
            Name = "Test Product",
            Brand = "Test Brand",
            Description = "A product for testing",
            Price = 9.99M,
            CategoryId = CategoryId,
            Category = new Category
            {
                Id = CategoryId,
                Name = "Test Category",
                Description = "A category for testing"
            },
            // IsActive = true
        };
        _context.Products.Add(product);
        
        await _context.SaveChangesAsync();
        return product;
    }

        private async Task<Product> SeedInactiveProduct()
    {
        Guid CategoryId = Guid.NewGuid();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Sku = "TESTSKU",
            Name = "Test Product",
            Brand = "Test Brand",
            Description = "A product for testing",
            Price = 9.99M,
            CategoryId = CategoryId,
            Category = new Category
            {
                Id = CategoryId,
                Name = "Test Category",
                Description = "A category for testing"
            },
            IsActive = false
        };
        _context.Products.Add(product);
        
        await _context.SaveChangesAsync();
        return product;
    }

    [Fact]
    public async Task GetProductByIdAsync_ActiveProductExists_ReturnsProduct()
    {
        // Arrange
        var seededProduct = await SeedActiveProduct();

        // Act
        var product = await _sut.GetProductByIdAsync(seededProduct.Id);

        // Assert
        Assert.Equal(seededProduct.Id, product.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_ProductNotFound_ThrowsProductIdNotFoundException()
    {
        // Act
        async Task act() => await _sut.GetProductByIdAsync(Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<ProductIdNotFoundException>(act);
    }

    [Fact]
    public async Task GetProductByIdAsync_InactiveProductExists_ThrowsProductIdNotFoundException()
    {
        // Arrange
        var inactiveProduct = await SeedInactiveProduct();

        // Act
        async Task act() => await _sut.GetProductByIdAsync(inactiveProduct.Id);

        // Assert
        await Assert.ThrowsAsync<ProductIdNotFoundException>(act);
    }

    [Fact]
    public async Task GetProductBySkuAsync_ActiveProductExists_ReturnsProduct()
    {
        // Arrange
        var seededProduct = await SeedActiveProduct();

        // Act
        var product = await _sut.GetProductBySkuAsync(seededProduct.Sku);

        // Assert
        Assert.Equal(seededProduct.Id, product.Id);
    }

    [Fact]
    public async Task GetProductBySkuAsync_ProductNotFound_ThrowsProductSkuNotFoundException()
    {
        // Act
        async Task act() => await _sut.GetProductBySkuAsync("NONEXISTENTSKU");

        // Assert
        await Assert.ThrowsAsync<ProductSkuNotFoundException>(act);
    }

    [Fact]
    public async Task GetProductBySkuAsync_InactiveProductExists_ThrowsProductSkuNotFoundException()
    {
        // Arrange
        var inactiveProduct = await SeedInactiveProduct();

        // Act
        async Task act() => await _sut.GetProductBySkuAsync(inactiveProduct.Sku);

        // Assert
        await Assert.ThrowsAsync<ProductSkuNotFoundException>(act);
    }


    [Fact]
    public async Task GetProductsAsync_ProductsExist_ReturnsActiveProductsOnly()
    {
        // Arrange
        var activeProduct1 = await SeedActiveProduct();
        var activeProduct2 = await SeedActiveProduct();
        var inactiveProduct = await SeedInactiveProduct();

        var query = new GridifyQuery
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _sut.GetProductsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result.Data, p => Assert.True(p.IsActive));
    }

    [Fact]
    public async Task AddProductAsync_SkuAvailable_CreatesProductAndReturnsIt()
    {
        // Arrange
        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Sku = "NEWSKU",
            Name = "New Product",
            Brand = "New Brand",
            Description = "A new product",
            Price = 19.99M,
            CategoryId = Guid.NewGuid(),
        };

        // Act
        var result = await _sut.AddProductAsync(newProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NEWSKU", result.Sku);

        var productInDb = await _context.Products.SingleAsync(p => p.Sku == "NEWSKU");
        Assert.Equal(result.Id, productInDb.Id);
    }

    [Fact]
    public async Task AddProductAsync_SkuUnavailable_ThrowsProductSkuAlreadyExistsException()
    {
        // Arrange
        await SeedActiveProduct();

        var newProduct = new Product
        {
            Sku = "TESTSKU",
            Name = "Duplicate Product",
            IsActive = true
        };

        // Act
        async Task act() => await _sut.AddProductAsync(newProduct);

        // Assert
        await Assert.ThrowsAsync<ProductSkuAlreadyExistsException>(act);
    }

    [Fact]
    public async Task UpdateProductAsync_ActiveProductExists_UpdatesFields()
    {
        // Arrange
        var product = await SeedActiveProduct();

        var updateDto = new ProductUpdateDto
        {
            Name = "Updated Name",
            Price = 29.99M
        };

        // Act
        var result = await _sut.UpdateProductAsync(product.Id, updateDto);

        // Assert
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(29.99M, result.Price);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateProductAsync_InactiveProductExists_UpdatesFields()
    {
        // Arrange
        var product = await SeedInactiveProduct();

        var updateDto = new ProductUpdateDto
        {
            Name = "Updated Inactive Product"
        };

        // Act
        var result = await _sut.UpdateProductAsync(product.Id, updateDto);

        // Assert
        Assert.Equal("Updated Inactive Product", result.Name);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateProductAsync_ProductDoesNotExist_ThrowsProductIdNotFoundException()
    {
        // Arrange
        var dto = new ProductUpdateDto { Name = "Does Not Matter" };

        // Act
        async Task act() => await _sut.UpdateProductAsync(Guid.NewGuid(), dto);

        // Assert
        await Assert.ThrowsAsync<ProductIdNotFoundException>(act);
    }

    [Fact]
    public async Task DeleteProductAsync_ProductExists_SoftDeletesProduct()
    {
        // Arrange
        var product = await SeedActiveProduct();

        // Act
        await _sut.DeleteProductAsync(product.Id);

        // Assert
        var deletedProduct = await _context.Products.FindAsync(product.Id);
        Assert.False(deletedProduct!.IsActive);
        Assert.NotNull(deletedProduct.DeletedAt);
    }

    [Fact]
    public async Task DeleteProductAsync_ProductDoesNotExist_ThrowsProductIdNotFoundException()
    {
        // Act
        async Task act() => await _sut.DeleteProductAsync(Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<ProductIdNotFoundException>(act);
    }

    [Fact]
    public async Task RestoreProductAsync_ProductExists_ReactivatesProduct()
    {
        // Arrange
        var product = await SeedInactiveProduct();

        // Act
        await _sut.RestoreProductAsync(product.Id);

        // Assert
        var restoredProduct = await _context.Products.FindAsync(product.Id);
        Assert.True(restoredProduct!.IsActive);
        Assert.Null(restoredProduct.DeletedAt);
        Assert.NotNull(restoredProduct.UpdatedAt);
    }

    [Fact]
    public async Task RestoreProductAsync_ProductDoesNotExist_ThrowsProductIdNotFoundException()
    {
        // Act
        async Task act() => await _sut.RestoreProductAsync(Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<ProductIdNotFoundException>(act);
    }
}
