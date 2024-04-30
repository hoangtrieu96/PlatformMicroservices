using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;

        int rabbitMQPort = 0;
        int.TryParse(_configuration["RabbitMQPort"], out rabbitMQPort);
        var factory = new ConnectionFactory() { 
            HostName = _configuration["RabbitMQHost"],
            Port = rabbitMQPort
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            
            Console.WriteLine("--> Connected to MessageBus");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the Message Bus: {ex}");
        }
    }

    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);

        if (_connection != null && _connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ connection open, sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection closed, not sending.");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "trigger",
                                routingKey: "",
                                basicProperties: null,
                                body: body);

        Console.WriteLine($"--> We have sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("MessageBus Disposed");
        if (_connection != null && _channel != null)
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine($"--> RabbitMQ Connection Shutdown");
    }
}
