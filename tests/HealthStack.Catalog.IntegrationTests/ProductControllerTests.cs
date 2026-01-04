using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HealthStack.Catalog.Api.DTOs;

namespace HealthStack.Catalog.IntegrationTests;

public class ProductControllerTests : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;

    public ProductControllerTests(CatalogApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new("Test");
    }

    private async Task<Guid> CreateCategoryAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/category",
            new CategoryCreateDto { Name = $"Supplements-{Guid.NewGuid()}", Description = "Medical Devices" });

        var category = await response.Content.ReadFromJsonAsync<CategoryReadDto>();
        return category!.Id;
    }

    [Fact]
    public async Task Create_And_Get_Product_By_Id()
    {
        var categoryId = await CreateCategoryAsync();

        var createDto = new ProductCreateDto
        {
            Sku = "THERMO-001",
            Name = "Thermometer",
            Brand = "HealthStack",
            Description = "Digital thermometer",
            Price = 15.99M,
            CategoryId = categoryId
        };

        var post = await _client.PostAsJsonAsync("/api/product", createDto);
        post.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await post.Content.ReadFromJsonAsync<ProductReadDto>();

        var get = await _client.GetAsync($"/api/product/{created!.Id}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_Product_By_Sku()
    {
        var categoryId = await CreateCategoryAsync();

        await _client.PostAsJsonAsync("/api/product", new ProductCreateDto
        {
            Sku = "MASK-001",
            Name = "Mask",
            Brand = "HealthStack",
            Description = "Medical mask",
            Price = 2.99M,
            CategoryId = categoryId
        });

        var response = await _client.GetAsync("/sku/MASK-001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_And_Restore_Product()
    {
        var categoryId = await CreateCategoryAsync();

        var create = await _client.PostAsJsonAsync("/api/product", new ProductCreateDto
        {
            Sku = "GLOVE-001",
            Name = "Gloves",
            Brand = "HealthStack",
            Description = "Medical gloves",
            Price = 5,
            CategoryId = categoryId
        });

        var product = await create.Content.ReadFromJsonAsync<ProductReadDto>();

        var delete = await _client.DeleteAsync($"/api/product/{product!.Id}");
        delete.StatusCode.Should().Be(HttpStatusCode.OK);

        var restore = await _client.PutAsync($"/api/product/{product.Id}/restore", null);
        restore.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
