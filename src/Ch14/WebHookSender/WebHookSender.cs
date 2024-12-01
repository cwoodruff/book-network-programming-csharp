using System.Text;
using System.Text.Json;

namespace WebHookSender;

public class WebHookSender
{
    private readonly string _endpointUrl;
    private readonly string _secret;
    
    private readonly HttpClient _httpClient;
    
    public WebHookSender(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public WebHookSender(string endpointUrl, string secret)
    {
        _httpClient = new HttpClient();
        _endpointUrl = endpointUrl;
        _secret = secret;
    }

    public async Task SendDataAsync(string eventType, object data)
    {
        var payload = new SenderPayload { EventType = eventType, Data = data };
        var payloadJson = JsonSerializer.Serialize(payload);
        
        var signature = SecurityHelpers.GenerateSignature(payloadJson, _secret);

        var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
        {
            Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-Signature", signature);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("WebHook sent successfully!");
        }
        else
        {
            Console.WriteLine($"Failed to send WebHook: {response.StatusCode}");
        }
    }
    
    public async Task SendPayloadAsync(SenderPayload payload)
    {
        var payloadJson = JsonSerializer.Serialize(payload);
        
        var signature = SecurityHelpers.GenerateSignature(payloadJson, _secret);
        
        var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
        {
            Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-Signature", signature);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to send WebHook: {response.StatusCode}");
        }
    }
}
