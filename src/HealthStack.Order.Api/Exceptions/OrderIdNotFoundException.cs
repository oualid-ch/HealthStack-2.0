namespace HealthStack.Order.Api.Exceptions;
public class OrderIdNotFoundException(Guid orderId) : Exception($"Order not found for Id {orderId}")
{
    public Guid OrderId { get; } = orderId;
}