using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Gridify;
using HealthStack.Catalog.Api.Controllers;
using HealthStack.Catalog.Api.DTOs;
using HealthStack.Catalog.Api.Models;
using HealthStack.Catalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HealthStack.Catalog.UnitTests;

public class ProductControllerTestsUnitTests
{
    private readonly Mock<IValidator<ProductCreateDto>> _productCreateValidator = new();
    private readonly Mock<IValidator<ProductUpdateDto>> _productUpdateValidator = new();
    private readonly Mock<IProductService> _productServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();


    private ProductController CreateController()
        => new(
            _productCreateValidator.Object,
            _productUpdateValidator.Object,
            _productServiceMock.Object,
            _mapperMock.Object
        );

    [Fact]
    public async Task GetProductById_ProductExists_ReturnsOk()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid() };
        var dto = new ProductReadDto { Id = product.Id };

        _productServiceMock
            .Setup(s => s.GetProductByIdAsync(product.Id))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(m => m.Map<ProductReadDto>(product))
            .Returns(dto);

        var controller = CreateController();

        // Act
        var result = await controller.GetProductById(product.Id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        ok.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetProductBySku_ProductExists_ReturnsOk()
    {
        var product = new Product { Id = Guid.NewGuid(), Sku = "SKU1" };
        var dto = new ProductReadDto { Id = product.Id };

        _productServiceMock
            .Setup(s => s.GetProductBySkuAsync("SKU1"))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(m => m.Map<ProductReadDto>(product))
            .Returns(dto);

        var controller = CreateController();

        var result = await controller.GetProductBySku("SKU1");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        ok.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkWithMappedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        var paging = new Paging<Product>
        {
            Data = products,
            Count = products.Count
        };

        var dtoList = products.Select(p => new ProductReadDto { Id = p.Id });

        _productServiceMock
            .Setup(s => s.GetProductsAsync(It.IsAny<GridifyQuery>()))
            .ReturnsAsync(paging);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<ProductReadDto>>(products))
            .Returns(dtoList);

        var controller = CreateController();

        // Act
        var result = await controller.GetProducts(new GridifyQuery());

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        ok.Value.Should().BeEquivalentTo(new { Data = dtoList });
    }


    [Fact]
    public async Task AddProduct_ValidModel_ReturnsCreated()
    {
        var dto = new ProductCreateDto { Name = "Test" };
        var product = new Product { Id = Guid.NewGuid() };
        var readDto = new ProductReadDto { Id = product.Id };

        _productCreateValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());

        _mapperMock
            .Setup(m => m.Map<Product>(dto))
            .Returns(product);

        _productServiceMock
            .Setup(s => s.AddProductAsync(product))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(m => m.Map<ProductReadDto>(product))
            .Returns(readDto);

        var controller = CreateController();

        var result = await controller.AddProduct(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        created.Value.Should().BeEquivalentTo(readDto);
    }

    [Fact]
    public async Task AddProduct_InvalidModel_ReturnsBadRequest()
    {
        var dto = new ProductCreateDto();

        _productCreateValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult(
                [ new ValidationFailure("Name", "Required") ]
            ));

        var controller = CreateController();

        var result = await controller.AddProduct(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProduct_ValidModel_ReturnsOk()
    {
        var dto = new ProductUpdateDto { Name = "Updated" };
        var product = new Product { Id = Guid.NewGuid(), Name = "Updated" };
        var readDto = new ProductReadDto { Id = product.Id };

        _productUpdateValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());

        _productServiceMock
            .Setup(s => s.UpdateProductAsync(product.Id, dto))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(m => m.Map<ProductReadDto>(product))
            .Returns(readDto);

        var controller = CreateController();

        var result = await controller.UpdateProduct(product.Id, dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        ok.Value.Should().BeEquivalentTo(readDto);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var controller = CreateController();

        var result = await controller.DeleteProduct(id);

        Assert.IsType<OkResult>(result);
        _productServiceMock.Verify(s => s.DeleteProductAsync(id), Times.Once);
    }

    [Fact]
    public async Task RestoreProduct_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var controller = CreateController();

        var result = await controller.RestoreProduct(id);

        Assert.IsType<OkResult>(result);
        _productServiceMock.Verify(s => s.RestoreProductAsync(id), Times.Once);
    }
}
