using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace MessageProducerApp;

public class MessageProducer : IDisposable, IAsyncDisposable
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly string _queueName;

    public MessageProducer(string hostName, string queueName)
    {
        _queueName = queueName;
        var factory = new ConnectionFactory { HostName = hostName };
        InitializeAsync(factory).GetAwaiter().GetResult();
    }

    private async Task InitializeAsync(ConnectionFactory factory)
    {
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(message);
            var props = new BasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = DeliveryModes.Persistent;
            await _channel.BasicPublishAsync(exchange: "", routingKey: _queueName, mandatory: false, basicProperties: props, body: body);
            Console.WriteLine($"[x] Sent: {message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[!] Error sending message: {ex.Message}");
            // Add additional error logging or handling here
        }
    }

    public void Dispose()
    {
        _channel.CloseAsync();
        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _connection.DisposeAsync();
    }
}