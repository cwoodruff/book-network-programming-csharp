using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:5001/chathub")
    .WithAutomaticReconnect([TimeSpan.Zero, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)])
    .Build();
    
try
{
    await connection.StartAsync();
    Console.WriteLine("Connection started successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");
}

connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.WriteLine($"{user}: {message}");
});

await connection.InvokeAsync("SendMessage", "User1", "Hello, SignalR!");

connection.Closed += async (error) =>
{
    Console.WriteLine($"Connection closed. Error: {error?.Message}");
    await Task.Delay(5000); // Wait before attempting to reconnect
    try
    {
        await connection.StartAsync();
        Console.WriteLine("Reconnection successful.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Reconnection failed: {ex.Message}");
    }
};