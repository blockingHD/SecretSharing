using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace SecretSharing.ServiceDefaults;

public class RabbitSetup : IEventLogger
{
    private readonly IConnection _connection;

    public RabbitSetup(IConnectionFactory connection)
    {
        _connection = connection.CreateConnection();
        
        Setup();
    }
    
    private void Setup()
    {
        using var channel = _connection.CreateModel();
        
        channel.ExchangeDeclare("events", ExchangeType.Topic, durable: true);
        channel.QueueDeclare("logging", durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind("logging", "events", "#");
    }
    
    public void PostMessage(string eventName, string message)
    {
        using var channel = _connection.CreateModel();
        
        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish("events", eventName, null, body);
    }
}