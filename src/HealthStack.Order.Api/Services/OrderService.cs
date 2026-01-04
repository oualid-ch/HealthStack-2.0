using Gridify;
using Gridify.EntityFramework;
using HealthStack.Order.Api.Data;
using HealthStack.Order.Api.Exceptions;
using HealthStack.Order.Api.Models;
using Microsoft.EntityFrameworkCore;
using HealthStack.Order.Api.Auth;
using HealthStack.Order.Api.Mapping;
using HealthStack.Order.Api.Clients;
// using HealthStack.Order.Api.DTOs;

namespace HealthStack.Order.Api.Services;

public class OrderService(
    AppDbContext context,
    ILogger<OrderService> logger,
    ICurrentUser currentUser,
    IProductClient _productClient
) : IOrderService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<OrderService> _logger = logger;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IProductClient _productClient = _productClient;


    public async Task<OrderEntry> GetMyOrderByIdAsync(Guid id)
    {
        var userId = _currentUser.UserId;

        var order = await _context.Orders
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        return order ?? throw new OrderIdNotFoundException(id);
    }

    public async Task<Paging<OrderEntry>> GetMyOrdersAsync(GridifyQuery query)
    {
        var mapper = new OrderEntryGridifyMapper();

        return await _context.Orders
            .Include(o => o.Items)
            .GridifyAsync(query, mapper);
    }

    public async Task<OrderEntry> CreateOrderAsync(OrderEntry order)
    {
        decimal total = 0m;

        foreach (var item in order.Items)
        {
            var product = await _productClient.GetProductByIdAsync(item.ProductId);
            item.ProductName = product.Name;
            item.UnitPrice = product.Price;
            total += product.Price * item.Quantity;
        }

        order.TotalAmount = total;
        order.Status = "Pending";
        order.CreatedAt = DateTime.UtcNow;
        order.UserId = (Guid)_currentUser.UserId!;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }
}