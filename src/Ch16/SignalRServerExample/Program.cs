using SignalRServerExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming connection: {context.Request.Path}");
    await next();
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub");
    endpoints.MapFallback(context =>
    {
        context.Response.StatusCode = 404;
        return context.Response.WriteAsync("Endpoint not found.");
    });
});

app.Run();