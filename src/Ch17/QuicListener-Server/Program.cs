using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Https.Internal;

if (!QuicListener.IsSupported)
{
    Console.WriteLine("QUIC is not supported");
    return;
}

var cert = CertificateLoader.LoadFromStoreCert("localhost", StoreName.My.ToString(), StoreLocation.CurrentUser, false);

var serverConnectionOptions = new QuicServerConnectionOptions
{
    DefaultStreamErrorCode = 0x0A,
    DefaultCloseErrorCode = 0x0B,
    ServerAuthenticationOptions = new SslServerAuthenticationOptions
    {
        ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
        ServerCertificate = CertificateLoader.LoadFromStoreCert("localhost", StoreName.My.ToString(), StoreLocation.CurrentUser, false)
    }
};

var listener = await QuicListener.ListenAsync(new QuicListenerOptions
{
    ListenEndPoint = new IPEndPoint(IPAddress.Loopback, 0),
    ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
    ConnectionOptionsCallback = (_, _, _) => ValueTask.FromResult(serverConnectionOptions)
});

var isRunning = true;

while (isRunning)
{
    var connection = await listener.AcceptConnectionAsync();
    Console.WriteLine($"Connection established with {connection.RemoteEndPoint}");
    _ = Task.Run(() => HandleConnectionAsync(connection));
}

await listener.DisposeAsync();


static async Task HandleConnectionAsync(QuicConnection connection)
{
    await using var stream = await connection.AcceptInboundStreamAsync();
    var buffer = new byte[1024];
    var bytesRead = await stream.ReadAsync(buffer);
    Console.WriteLine($"Received: {Encoding.UTF8.GetString(buffer, 0, bytesRead)}");

    await stream.WriteAsync(Encoding.UTF8.GetBytes("Hello from server!"));
    await connection.CloseAsync(0);
}