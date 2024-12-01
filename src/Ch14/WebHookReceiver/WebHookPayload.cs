namespace WebHookReceiver;

public class WebHookPayload
{
    public string? EventType { get; set; }
    public object? Data { get; set; }
    public DateTime ReceivedAt { get; set; }
}
