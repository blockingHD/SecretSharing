using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecretSharing.ServiceDefaults.Attribute;

namespace SecretSharing.Worker;

public class Worker : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;
    private IModel Channel { get; set; }

    public Worker(IConnection connection, IServiceScopeFactory scopeFactory, ILogger<Worker> logger)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }

        Channel = _connection.CreateModel();
        var consumer = new EventingBasicConsumer(Channel);

        consumer.Received += (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Received message: {message}", message);
            }
            
            var log = JsonSerializer.Deserialize<EventLoggerBody>(message);

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LoggerDbContext>();
            
            dbContext.Logs.Add(new Log
            {
                EventId = eventArgs.RoutingKey,
                UserId = log.UserId,
                Path = log.Path
            });
            
            dbContext.SaveChanges();

            Channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        Channel.BasicConsume("logging", false, consumer);
    }
}