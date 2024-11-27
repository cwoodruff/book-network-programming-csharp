using System.Text;
using MQTTnet.Protocol;
using MQTTnet;
using MQTTnet.Client;

namespace MQTTClient;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting MQTT Client...");

        // Define the MQTT client options
        var options = new MqttClientOptionsBuilder()
            .WithClientId("Client1")
            .WithTcpServer("localhost", 1883) // Connect to the MQTT server
            .WithCredentials("testuser", "testpassword") // Use valid credentials
            .WithCleanSession()
            .Build();

        // Create the MQTT client
        var mqttClient = new MqttFactory().CreateMqttClient();

        // Handlers
        mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("Connected to the broker!");

            // Subscribe to a topic
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder() //FIX
                .WithTopic("test/topic")
                .Build());

            Console.WriteLine("Subscribed to topic 'test/topic'.");
        };

        mqttClient.DisconnectedAsync += e =>
        {
            Console.WriteLine("Disconnected from the broker.");
            return Task.CompletedTask;
        };

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine($"Message received on topic {e.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
            return Task.CompletedTask;
        };

        // Connect to the MQTT server
        await mqttClient.ConnectAsync(options);
        Console.WriteLine("MQTT Client is running. Press any key to publish a message.");

        Console.ReadKey();

        // Publish a message
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("test/topic")
            .WithPayload("Hello MQTT!")
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();

        await mqttClient.PublishAsync(message);
        Console.WriteLine("Message published!");

        Console.WriteLine("Press any key to disconnect...");
        Console.ReadKey();

        // Disconnect from the MQTT server
        await mqttClient.DisconnectAsync();
        Console.WriteLine("MQTT Client disconnected.");
    }
}