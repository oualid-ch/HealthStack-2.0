using HealthStack.Notification.Worker;
using HealthStack.Notification.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<OrderCreatedConsumer>();

var host = builder.Build();
host.Run();
