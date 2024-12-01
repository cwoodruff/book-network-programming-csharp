namespace WebHookSender;

class Program
{
    static async Task Main(string[] args)
    {
        var sender = new WebHookSender("https://localhost:5001/api/webhooks", "your_secret_key");
        await sender.SendDataAsync("event_type", new { message = "Hello, WebHook!" });
        
        SenderPayload payload = new SenderPayload() {EventType = "event_type", Data = "Hello, WebHook!"};
        await sender.SendPayloadAsync(payload);
    }
}