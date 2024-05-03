using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class SecureMultithreadedServer(string ipAddress, int port)
{
    private TcpListener listener;
    private volatile bool isRunning;
    private readonly IPAddress ipAddress = IPAddress.Parse(ipAddress);
    private readonly ConcurrentDictionary<int, TcpClient> clients = new ConcurrentDictionary<int, TcpClient>();
    private static readonly object _lock = new object();

    private void Start()
    {
        listener = new TcpListener(ipAddress, port);
        listener.Start();
        isRunning = true;
        Console.WriteLine("Server started...");

        while (isRunning)
        {
            Console.WriteLine("Waiting for connections...");
            var client = listener.AcceptTcpClient();
            var clientId = client.GetHashCode();
            clients.TryAdd(clientId, client);

            Thread clientThread = new Thread(() => HandleClient(client, clientId));
            clientThread.Start();
        }
    }

    private void HandleClient(TcpClient client, int clientId)
    {
        try
        {
            using var networkStream = client.GetStream();
            using var reader = new StreamReader(networkStream, Encoding.UTF8);
            using var writer = new StreamWriter(networkStream, Encoding.UTF8);
            string? message = null;
            while (!string.IsNullOrEmpty(message = reader.ReadLine()))
            {
                Console.WriteLine($"Received from {clientId}: {message}");
                writer.WriteLine($"Echo {clientId}: {message}");
                writer.Flush();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with client {clientId}: {ex.Message}");
        }
        finally
        {
            client.Close();
            clients.TryRemove(clientId, out _);
            Console.WriteLine($"Client {clientId} disconnected.");
        }
    }

    public void Stop()
    {
        isRunning = false;
        listener.Stop();
        foreach (var client in clients.Values)
        {
            client.Close();
        }
        Console.WriteLine("Server stopped.");
    }

    public static void Main(string[] args)
    {
        var server = new SecureMultithreadedServer("127.0.0.1", 13000);
        server.Start();
    }
}