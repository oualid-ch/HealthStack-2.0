namespace HealthStack.Order.Api.DTOs;

public class OrderItemCreateDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}