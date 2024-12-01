using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

const int BufferSize = 1024 * 4;

List<WebSocket> connectedClients = new List<WebSocket>();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/signal", async context =>
    {
        await HandleWebSocketRequest(context);
    });
});

app.Run();

async Task HandleWebSocketRequest(HttpContext context)
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketSignaling(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
}

async Task HandleWebSocketSignaling(WebSocket webSocket)
{
    connectedClients.Add(webSocket);
    var buffer = new byte[1024 * 4];

    try
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            
            // Parsing JSON message to differentiate message types
            var signalingMessage = JsonSerializer.Deserialize<SignalingMessage>(message);
            
            switch (signalingMessage.Type)
            {
                case "offer":
                case "answer":
                case "candidate":
                    await ForwardSignalingMessage(signalingMessage, webSocket);
                    break;
                default:
                    Console.WriteLine($"Unknown message type: {signalingMessage.Type}");
                    break;
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        connectedClients.Remove(webSocket);
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }
}

async Task ForwardSignalingMessage(SignalingMessage signalingMessage, WebSocket sender)
{
    foreach (var client in connectedClients)
    {
        if (client != sender && client.State == WebSocketState.Open)
        {
            var messageJson = JsonSerializer.Serialize(signalingMessage);
            await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageJson)), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

public class SignalingMessage
{
    public string Type { get; set; }
    public string Payload { get; set; }
}