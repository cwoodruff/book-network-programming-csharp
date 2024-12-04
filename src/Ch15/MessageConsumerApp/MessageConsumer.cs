using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessageConsumerApp;

public class MessageConsumer : IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queueName;

    public MessageConsumer(string hostName, string queueName)
    {
        _queueName = queueName;
        var factory = new ConnectionFactory { HostName = hostName };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    public void StartListening()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[x] Received: {message}");
            // Simulate processing
            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
        Console.WriteLine("Consumer listening for messages...");
    }

    public void Dispose()
    {
        _channel.CloseAsync();
        _connection.CloseAsync();
    }
}