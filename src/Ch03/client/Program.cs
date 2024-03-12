using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    public static void Main()
    {
        ExecuteClient();
    }

    static void ExecuteClient()
    {
        try
        {
            // Create a TcpClient.
            // The client requires a TcpServer that is connected
            // to 127.0.0.1 on port 13000.
            TcpClient client = new TcpClient("127.0.0.1", 13000);

            // Translate the passed message into ASCII and store it as a Byte array.
            String data = "Hello Server!";
            Byte[] bytes = Encoding.ASCII.GetBytes(data);

            // Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(bytes, 0, bytes.Length);

            Console.WriteLine("Sent: {0}", data);

            // Buffer to store the response bytes.
            bytes = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 i = stream.Read(bytes, 0, bytes.Length);
            responseData = Encoding.ASCII.GetString(bytes, 0, i);
            Console.WriteLine("Received: {0}", responseData);

            // Close everything.
            stream.Close();
            client.Close();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\n Press any key to continue...");
        Console.ReadKey();
    }
}