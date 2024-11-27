using System.Text;
using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Server;

namespace MQTTServer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting MQTT Server...");

        // Create the MQTT server options
        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883);

        // Create and start the MQTT server
        var mqttServer = new MqttFactory(new ConsoleLogger()).CreateMqttServer(optionsBuilder.Build());

        await mqttServer.StartAsync();
        Console.WriteLine("MQTT Server is running on port 1883.");

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        await mqttServer.StopAsync();
        Console.WriteLine("MQTT Server stopped.");
    }
}
    
class ConsoleLogger : IMqttNetLogger
{
    readonly object _consoleSyncRoot = new();

    public bool IsEnabled => true;

    public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
    {
        var foregroundColor = ConsoleColor.White;
        switch (logLevel)
        {
            case MqttNetLogLevel.Verbose:
                foregroundColor = ConsoleColor.White;
                break;

            case MqttNetLogLevel.Info:
                foregroundColor = ConsoleColor.Green;
                break;

            case MqttNetLogLevel.Warning:
                foregroundColor = ConsoleColor.DarkYellow;
                break;

            case MqttNetLogLevel.Error:
                foregroundColor = ConsoleColor.Red;
                break;
        }

        if (parameters?.Length > 0)
        {
            message = string.Format(message, parameters);
        }

        lock (_consoleSyncRoot)
        {
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message);

            if (exception != null)
            {
                Console.WriteLine(exception);
            }
        }
    }
}