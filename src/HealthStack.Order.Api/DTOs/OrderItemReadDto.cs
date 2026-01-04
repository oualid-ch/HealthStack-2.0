namespace HealthStack.Order.Api.DTOs;

public class OrderItemReadDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } 
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}