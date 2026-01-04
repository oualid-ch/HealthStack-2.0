namespace HealthStack.Order.Api.DTOs;
public class OrderCreateDto
{
    public List<OrderItemCreateDto> Items { get; set; } = [];
}