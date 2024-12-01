using System.Security.Cryptography;
using System.Text;

namespace WebHookReceiver;

public static class SecurityHelpers
{
    public static bool ValidateSignature(string payload, string signature, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = Convert.ToBase64String(hash);
        return signature == computedSignature;
    }
    
    private static bool IsValidRequest(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-Signature", out var signature))
        {
            return false;
        }

        // Compute the expected signature using a secret key
        var secret = "your_secret_key";
        var payload = new StreamReader(request.Body).ReadToEnd();
        var expectedSignature = ComputeHmacSha256(payload, secret);

        return signature == expectedSignature;
    }

    private static string ComputeHmacSha256(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }
    
    private static bool IsRecentRequest(string timestampHeader)
    {
        if (DateTime.TryParse(timestampHeader, out var timestamp))
        {
            return (DateTime.UtcNow - timestamp).TotalMinutes <= 5;
        }

        return false;
    }
}
