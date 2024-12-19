using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

var builder = WebApplication.CreateBuilder(args);

var cert = CertificateLoader.LoadFromStoreCert("localhost", StoreName.My.ToString(), StoreLocation.CurrentUser, false);

// Configure Kestrel to support HTTP/3, HTTP/2, and HTTP/1.1
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseConnectionLogging();
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps(httpsOptions =>
        {
            httpsOptions.ServerCertificateSelector = (context, host) => cert;
        });
    });
});

var app = builder.Build();

// Define a simple endpoint to respond with HTTP version and message
app.MapGet("/process", async context =>
{
    var protocol = context.Request.Protocol; // Retrieve HTTP protocol version
    await context.Response.WriteAsync($"Hello! You are using {protocol}");
});

app.Run();