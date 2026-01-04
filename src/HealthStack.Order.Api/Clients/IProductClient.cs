using HealthStack.Order.Api.SharedDTOs;

namespace HealthStack.Order.Api.Clients;

public interface IProductClient
{
    Task<ProductReadDto> GetProductByIdAsync(Guid productId);
}
