using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace WebHookReceiver.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebHookController : ControllerBase
{
    
    private readonly ILogger<WebHookController> _logger;

    public WebHookController(ILogger<WebHookController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost]
    public IActionResult HandleWebHook([FromBody] WebHookPayload payload)
    {
        _logger.LogInformation("Received WebHook request. Headers: {Headers}, Payload: {Payload}",
            Request.Headers, payload);

        if (payload == null)
        {
            _logger.LogError("Payload is null");
            return BadRequest("Invalid payload");
        }
        
        // Process the payload based on the event type
        switch (payload.EventType)
        {
            case "payment_success":
                ProcessPaymentSuccess(payload);
                break;
            case "subscription_updated":
                ProcessSubscriptionUpdate(payload);
                break;
            default:
                return BadRequest("Unknown event type");
        }

        _logger.LogInformation("Processing event: {EventType}", payload.EventType);
        return Ok();
    }

    private void ProcessPaymentSuccess(WebHookPayload payload)
    {
        // Custom logic for handling payment success
        Console.WriteLine($"Payment received: {payload.Data}");
    }

    private void ProcessSubscriptionUpdate(WebHookPayload payload)
    {
        // Custom logic for handling subscription updates
        Console.WriteLine($"Subscription updated: {payload.Data}");
    }
    
    private async Task SaveEventToDatabase(WebHookPayload payload)
    {
        using var context = new AppDbContext();
        context.WebHookPayloads.Add(new WebHookPayload
        {
            EventType = payload.EventType,
            Data = payload.Data,
            ReceivedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}

public class AppDbContext : DbContext
{
    public DbSet<WebHookPayload> WebHookPayloads { get; set; }
}


