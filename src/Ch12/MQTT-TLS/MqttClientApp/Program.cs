using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttClientApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting MQTT Client...");

        // Load client certificate
        string certPath = "path_to_client_cert.pfx"; // Provide the path to your .pfx certificate file
        string certPassword = "your_cert_password";  // Provide the certificate password
        var clientCertificate = new X509Certificate2(certPath, certPassword);

        // Create MQTT client
        string brokerAddress = "your_broker_address"; // Replace with your MQTT server's address
        int brokerPort = 8883;

        var mqttClient = new MqttClient(
            brokerAddress,
            brokerPort,
            true,
            MqttSslProtocols.TLSv1_2,
            (sender, certificate, chain, sslPolicyErrors) => true,
            (sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) => clientCertificate);

        mqttClient.MqttMsgPublishReceived += (sender, e) =>
        {
            Console.WriteLine($"Message Received: {Encoding.UTF8.GetString(e.Message)} on Topic {e.Topic}");
        };

        mqttClient.ConnectionClosed += (sender, e) =>
        {
            Console.WriteLine("Disconnected from the broker.");
        };

        string clientId = Guid.NewGuid().ToString();
        mqttClient.Connect(clientId);
        Console.WriteLine("Connected to MQTT Broker!");

        // Subscribe to a topic
        string topic = "test/topic";
        mqttClient.Subscribe([topic], [MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE]);
        Console.WriteLine($"Subscribed to topic: {topic}");

        // Publish a message
        string message = "Hello MQTT with TLS!";
        mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message));
        Console.WriteLine($"Published message: {message}");

        Console.WriteLine("Press any key to disconnect...");
        Console.ReadKey();

        mqttClient.Disconnect();
        Console.WriteLine("MQTT Client disconnected.");
    }
}