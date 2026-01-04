using Gridify;
using HealthStack.Order.Api.Models;

namespace HealthStack.Order.Api.Services;
public interface IOrderService
{
    public Task<OrderEntry> GetMyOrderByIdAsync(Guid id);
    public Task<Paging<OrderEntry>> GetMyOrdersAsync(GridifyQuery query);
    public Task<OrderEntry> CreateOrderAsync(OrderEntry order);
    // public Task UpdateOrderStatusAsync(Guid id, string status);
}