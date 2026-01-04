namespace HealthStack.Order.Api.Messaging;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "rabbitmq";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "order.exchange";
}
