using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
        ServerCertificate =
            CertificateLoader.LoadFromStoreCert("localhost", StoreName.My.ToString(), StoreLocation.CurrentUser, false)
    }
};

var listener = await QuicListener.ListenAsync(new QuicListenerOptions
{
    ListenEndPoint = new IPEndPoint(IPAddress.Loopback, 0),
    ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
    // Callback to provide options for the incoming connections, it gets called once per each connection.
    ConnectionOptionsCallback = (_, _, _) => ValueTask.FromResult(serverConnectionOptions)
});

var clientConnectionOptions = new QuicClientConnectionOptions
{
    RemoteEndPoint = listener.LocalEndPoint,
    DefaultStreamErrorCode = 0x0A,
    DefaultCloseErrorCode = 0x0B,

    MaxInboundUnidirectionalStreams = 10,
    MaxInboundBidirectionalStreams = 100,

    ClientAuthenticationOptions = new SslClientAuthenticationOptions
    {
        ApplicationProtocols = [new SslApplicationProtocol("protocol-name")],
        TargetHost = ""
    }
};

var connection = await QuicConnection.ConnectAsync(clientConnectionOptions);

Console.WriteLine($"Connected {connection.LocalEndPoint} --> {connection.RemoteEndPoint}");

var outgoingStream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);

var isRunning = true;

while (isRunning)
{
    Console.WriteLine($"New connection established with {connection.RemoteEndPoint}");
    var quicStream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);
    _ = Task.Run(() => WorkWithStreamAsync(quicStream));
}

await connection.CloseAsync(0x0C);

await connection.DisposeAsync();

async Task WorkWithStreamAsync(Stream stream)
{
    await using (stream)
    {
        byte[] buffer = new byte[1024];
        int count = 0;
        while ((count = await stream.ReadAsync(buffer)) > 0)
        {
            await stream.WriteAsync(buffer.AsMemory(0, count));
        }
    }
}