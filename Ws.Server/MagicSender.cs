using System.Net.WebSockets;
using System.Text;

namespace Ws.Server
{
    public static class MagicSender
    {
        public static async Task Send(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if(result != null)
            {
                while (!result.CloseStatus.HasValue)
                {
                    string msg = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
                    Console.WriteLine($"Client says: {msg}");
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Server says: {DateTime.UtcNow:f}")),
                        result.MessageType,
                        result.EndOfMessage,
                        CancellationToken.None);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
