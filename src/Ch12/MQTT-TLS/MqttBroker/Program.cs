using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using MQTTnet;
using MQTTnet.Server;

namespace MqttBroker;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting MQTT Server...");

        // Load server certificate
        string certPath = "path_to_server_cert.pfx"; // Provide the path to your .pfx certificate file
        string certPassword = "your_cert_password";  // Provide the certificate password
        var serverCertificate = new X509Certificate2(certPath, certPassword);

        // Create MQTT broker
        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithEncryptedEndpoint()
            .WithEncryptedEndpointPort(8883)
            .WithEncryptionCertificate(serverCertificate.Export(X509ContentType.Pfx))
            .WithEncryptionSslProtocol(SslProtocols.Tls12)
            .Build();

        var mqttServer = new MqttFactory().CreateMqttServer(mqttServerOptions);
        mqttServer.ClientConnectedAsync += (e) =>
        {
            Console.WriteLine($"Client Connected: " + e.ClientId);
            return Task.CompletedTask;
        };
        mqttServer.ClientDisconnectedAsync += (e) =>
        {
            Console.WriteLine("Client Disconnected: " + e.ClientId);
            return Task.CompletedTask;
        };

        // Start the server
        mqttServer.StartAsync();
        Console.WriteLine("MQTT Server started on port 8883 with TLS enabled.");

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();
        mqttServer.StopAsync();
        Console.WriteLine("MQTT Server stopped.");
    }
}