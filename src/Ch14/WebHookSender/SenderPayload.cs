namespace WebHookSender;

public class SenderPayload
{
    public string? EventType { get; set; }
    public object? Data { get; set; }
}
