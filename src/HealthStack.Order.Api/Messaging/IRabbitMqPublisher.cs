namespace HealthStack.Order.Api.Messaging;

public interface IRabbitMqPublisher
{
    void Publish<T>(T message, string routingKey);
}
