var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var allowedIPs = new[] { "192.168.1.1", "203.0.113.10" };
    var remoteIp = context.Connection.RemoteIpAddress?.ToString();

    if (!allowedIPs.Contains(remoteIp))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return;
    }

    await next();
});

app.MapControllers();
app.Run();