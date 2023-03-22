using System.Net;
using System.Net.WebSockets;
using Ws.Server;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var wsOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) };
app.UseWebSockets(wsOptions);
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/send")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
            {
                await MagicSender.Send(context, webSocket);
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    await next();
});

app.Run();
