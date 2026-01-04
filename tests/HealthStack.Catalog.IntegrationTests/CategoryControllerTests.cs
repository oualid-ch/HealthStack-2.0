using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HealthStack.Catalog.Api.DTOs;

namespace HealthStack.Catalog.IntegrationTests;

public class CategoryControllerTests : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;

    public CategoryControllerTests(CatalogApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new("Test");
    }

    [Fact]
    public async Task Create_And_Get_Categories()
    {
        // Arrange
        var createDto = new CategoryCreateDto
        {
            Name = "Medicines",
            Description = "All kinds of medicines",
        };

        // Act
        var postResponse = await _client.PostAsJsonAsync("/api/category", createDto);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await _client.GetAsync("/api/category");
        var categories = await getResponse.Content.ReadFromJsonAsync<List<CategoryReadDto>>();

        // Assert
        categories.Should().ContainSingle(c => c.Name == "Medicines");
    }

    [Fact]
    public async Task Delete_And_Restore_Category()
    {
        var createDto = new CategoryCreateDto
        {
            Name = $"Supplements-{Guid.NewGuid()}",
            Description = "Vitamins and more"
        };
        var create = await _client.PostAsJsonAsync("/api/category", createDto);
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var category = await create.Content.ReadFromJsonAsync<CategoryReadDto>();

        // delete
        var delete = await _client.DeleteAsync($"/api/category/{category!.Id}");
        delete.StatusCode.Should().Be(HttpStatusCode.OK);

        // restore
        var restore = await _client.PutAsync($"/api/category/{category.Id}/restore", null);
        restore.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
