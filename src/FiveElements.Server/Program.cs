using FiveElements.Server.Services;
using FiveElements.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register custom services
builder.Services.AddSingleton<IGameLogicService, GameLogicService>();
builder.Services.AddSingleton<IGameWorldService, GameWorldService>();
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddHostedService<MonsterMovementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseWebSockets();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// WebSocket endpoint
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var connectionManager = context.RequestServices.GetRequiredService<IConnectionManager>();
            await connectionManager.HandleConnectionAsync(webSocket, context);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

app.Run();