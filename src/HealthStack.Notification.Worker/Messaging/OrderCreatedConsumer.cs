using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using HealthStack.Notification.Worker.Events;

namespace HealthStack.Notification.Worker.Messaging;

public class OrderCreatedConsumer(
    ILogger<OrderCreatedConsumer> logger,
    IOptions<RabbitMqOptions> options) : BackgroundService
{
    private readonly ILogger<OrderCreatedConsumer> _logger = logger;
    private readonly RabbitMqOptions _options = options.Value;
    private IConnection? _connection;
    private IModel? _channel;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true
        );

        _channel.QueueDeclare(
            queue: "notification.order.created.queue",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        _channel.QueueBind(
            queue: "notification.order.created.queue",
            exchange: _options.ExchangeName,
            routingKey: "order.created"
        );

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, args) =>
        {
            var json = Encoding.UTF8.GetString(args.Body.ToArray());
            var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(json);

            _logger.LogInformation(
                "[EMAIL] Order confirmation sent for Order {OrderId} (User {UserId}, Total {Total})",
                evt!.OrderId,
                evt.UserId,
                evt.TotalAmount
            );
        };

        _channel.BasicConsume(
            queue: "notification.order.created.queue",
            autoAck: true,
            consumer: consumer
        );

        _logger.LogInformation("Notification service listening for OrderCreated events");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
