using System.Net;

using var client = new HttpClient();
client.DefaultRequestVersion = HttpVersion.Version30;

Console.WriteLine("--- localhost:5001 ---");

// The client falls back to HTTP2 or HTTP1 if HTTP3 is not supported
client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;

// Will use HTTP3 if the server supports it
var resp = await client.GetAsync("https://localhost:5001/");
string body = await resp.Content.ReadAsStringAsync();

Console.WriteLine(
    $"status: {resp.StatusCode}, version: {resp.Version}, " +
    $"body: {body.Substring(0, Math.Min(100, body.Length))}");