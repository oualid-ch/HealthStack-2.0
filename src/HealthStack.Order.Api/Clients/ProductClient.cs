using System.Net;
using HealthStack.Order.Api.Exceptions;
using HealthStack.Order.Api.SharedDTOs;

namespace HealthStack.Order.Api.Clients;

public class ProductClient(HttpClient httpClient) : IProductClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<ProductReadDto> GetProductByIdAsync(Guid productId)
    {
        var response = await _httpClient.GetAsync($"/api/Product/{productId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new ProductNotFoundException(productId);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProductReadDto>() 
            ?? throw new ProductApiException(productId);
    }
}