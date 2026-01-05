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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            DispatchConsumersAsync = true
        };

        // Retry until RabbitMQ is ready
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _logger.LogInformation("Connected to RabbitMQ");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "RabbitMQ not ready yet. Retrying in 5 seconds. Error: {Message}",
                    ex.Message
                );

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        if (_channel is null || stoppingToken.IsCancellationRequested)
            return;

        // Exchange
        _channel.ExchangeDeclare(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true
        );

        // Queue
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

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(json);

                if (evt is null)
                    return;

                _logger.LogInformation(
                    "[EMAIL] Order confirmation sent for Order {OrderId} (User {UserId}, Total {Total})",
                    evt.OrderId,
                    evt.UserId,
                    evt.TotalAmount
                );

                _channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process OrderCreated event");

                // Requeue message
                _channel.BasicNack(args.DeliveryTag, false, requeue: true);
            }

            await Task.CompletedTask;
        };

        _channel.BasicConsume(
            queue: "notification.order.created.queue",
            autoAck: false,
            consumer: consumer
        );

        _logger.LogInformation("Notification service listening for OrderCreated events");

        // Keep the worker alive
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        try
        {
            _channel?.Close();
            _connection?.Close();
        }
        catch
        {
            // ignore shutdown errors
        }

        base.Dispose();
    }
}
