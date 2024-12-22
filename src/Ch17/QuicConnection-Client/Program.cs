using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https.Internal;

try
{
    bool isRunning = true;
    Console.WriteLine("Starting QUIC client...");

    // First, check if QUIC is supported.
    if (!QuicConnection.IsSupported)
    {
        Console.WriteLine("QUIC is not supported, check for presence of libmsquic and support of TLS 1.3.");
        return;
    }

    // Share configuration for each incoming connection.
    // This represents the minimal configuration necessary.
    var serverConnectionOptions = new QuicServerConnectionOptions
    {
        // Used to abort stream if it's not properly closed by the user.
        // See https://www.rfc-editor.org/rfc/rfc9000#section-20.2
        DefaultStreamErrorCode = 0x0A, // Protocol-dependent error code.

        // Used to close the connection if it's not done by the user.
        // See https://www.rfc-editor.org/rfc/rfc9000#section-20.2
        DefaultCloseErrorCode = 0x0B, // Protocol-dependent error code.

        // Same options as for server side SslStream.
        ServerAuthenticationOptions = new SslServerAuthenticationOptions
        {
            // Specify the application protocols that the server supports. This list must be a subset of the protocols specified in QuicListenerOptions.ApplicationProtocols.
            ApplicationProtocols = [new SslApplicationProtocol("protocol1")],
            // Server certificate, it can also be provided via ServerCertificateContext or ServerCertificateSelectionCallback.
            ServerCertificate = CertificateLoader.LoadFromStoreCert("localhost", StoreName.My.ToString(),
                StoreLocation.CurrentUser, false)
        }
    };
    // Initialize, configure the listener and start listening.
    var listener = await QuicListener.ListenAsync(new QuicListenerOptions
    {
        // Define the endpoint on which the server will listen for incoming connections. The port number 0 can be replaced with any valid port number as needed.
        ListenEndPoint = new IPEndPoint(IPAddress.Loopback, 0),
        // List of all supported application protocols by this listener.
        ApplicationProtocols = [new SslApplicationProtocol("protocol1")],
        // Callback to provide options for the incoming connections, it gets called once per each connection.
        ConnectionOptionsCallback = (_, _, _) => ValueTask.FromResult(serverConnectionOptions)
    });
    // This represents the minimal configuration necessary to open a connection.
    var clientConnectionOptions = new QuicClientConnectionOptions
    {
        // End point of the server to connect to.
        RemoteEndPoint = listener.LocalEndPoint,

        // Used to abort stream if it's not properly closed by the user.
        // See https://www.rfc-editor.org/rfc/rfc9000#section-20.2
        DefaultStreamErrorCode = 0x0A, // Protocol-dependent error code.

        // Used to close the connection if it's not done by the user.
        // See https://www.rfc-editor.org/rfc/rfc9000#section-20.2
        DefaultCloseErrorCode = 0x0B, // Protocol-dependent error code.

        // Optionally set limits for inbound streams.
        MaxInboundUnidirectionalStreams = 10,
        MaxInboundBidirectionalStreams = 100,

        // Same options as for client side SslStream.
        ClientAuthenticationOptions = new SslClientAuthenticationOptions
        {
            // List of supported application protocols.
            ApplicationProtocols = [new SslApplicationProtocol("protocol1")],
            // The name of the server the client is trying to connect to. Used for server certificate validation.
            TargetHost = "localhost",
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        }
    };

    // Initialize, configure and connect to the server.
    var connection = await QuicConnection.ConnectAsync(clientConnectionOptions);

    Console.WriteLine($"Connected {connection.LocalEndPoint} --> {connection.RemoteEndPoint}");

    // Open a bidirectional (can both read and write) outbound stream.
    // Opening a stream reserves it but does not notify the peer or send any data. If you don't send data, the peer
    // won't be informed about the stream, which can cause AcceptInboundStreamAsync() to hang. To avoid this, ensure
    // you send data on the stream to properly initiate communication.
    var outgoingStream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);

    // Work with the outgoing stream ...

    // To accept any stream on a client connection, at least one of MaxInboundBidirectionalStreams or MaxInboundUnidirectionalStreams of QuicConnectionOptions must be set.
    while (isRunning)
    {
        // Accept an inbound stream.
        var incomingStream = await connection.AcceptInboundStreamAsync();

        // Work with the incoming stream ...
        byte[] buffer = new byte[1024];
        await incomingStream.ReadExactlyAsync(buffer);
        string recoveredString = System.Text.Encoding.UTF8.GetString(buffer);

        Console.WriteLine(recoveredString + "Received: " + DateTime.UtcNow.ToString());
    }

    // Close the connection with the custom code.
    await connection.CloseAsync(0x0C);

    // Dispose the connection.
    await connection.DisposeAsync();
}
catch (Exception ex)
{
    throw new Exception(ex.Message, ex);
}