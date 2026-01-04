using Gridify;
using HealthStack.Order.Api.Models;

namespace HealthStack.Order.Api.Mapping;

public class OrderEntryGridifyMapper : GridifyMapper<OrderEntry>
{
    public OrderEntryGridifyMapper()
    {
        AddMap("totalAmount", o => o.TotalAmount);
        AddMap("status", o => o.Status);
        AddMap("createdAt", p => p.CreatedAt);
    }
}