using System.Net.Sockets;
using System.Text;

namespace Client;

class TcpClientExample
{
    static void Main(string[] args)
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port combination.
            int port = 13000;
            TcpClient client = new TcpClient("127.0.0.1", port);

            // Translate the passed message into ASCII and store it as a byte array.
            string messageToSend = "Hello from the client!";
            byte[] data = Encoding.ASCII.GetBytes(messageToSend);

            // Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);
            Console.WriteLine($"Sent: {messageToSend}");

            // Receive the TcpServer.response.
            // Buffer to store the response bytes.
            data = new byte[256];
            // String to store the response ASCII representation.
            string responseData;

            // Read the first batch of the TcpServer response bytes.
            int bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine($"Received: {responseData}");

            // Close everything.
            stream.Close();
            client.Close();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine($"ArgumentNullException: {e}");
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e}");
        }

        Console.WriteLine("\n Press Enter to continue...");
        Console.ReadLine();
    }
}