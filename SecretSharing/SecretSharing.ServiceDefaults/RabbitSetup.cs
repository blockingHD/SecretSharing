using System.Text;
using RabbitMQ.Client;

namespace Microsoft.Extensions.Hosting;

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
    }
    
    public void PostMessage(string eventName, string message)
    {
        using var channel = _connection.CreateModel();
        
        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish("events", eventName, null, body);
    }
}