using System.Net.Quic;
using System.Net;
using System.Net.Security;
using System.Text;

var connectionOptions = new QuicClientConnectionOptions
{
    RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, 5001),
    ClientAuthenticationOptions = new SslClientAuthenticationOptions
    {
        ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 }
    }
};

await using var connection = await QuicConnection.ConnectAsync(connectionOptions);
Console.WriteLine("Connected to server using QuicConnection.");

await using var stream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);
await stream.WriteAsync(Encoding.UTF8.GetBytes("Hello from QuicConnection client!"));

var buffer = new byte[1024];
var bytesRead = await stream.ReadAsync(buffer);
Console.WriteLine($"Server response: {Encoding.UTF8.GetString(buffer, 0, bytesRead)}");

await connection.CloseAsync(0);


