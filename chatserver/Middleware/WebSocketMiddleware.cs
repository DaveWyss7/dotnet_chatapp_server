using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ChatServer.Middleware;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await _next(context);
            return;
        }

        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var socketId = Guid.NewGuid().ToString();
        _sockets.TryAdd(socketId, webSocket);

        try
        {
            await HandleWebSocketAsync(socketId, webSocket);
        }
        finally
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket closed", CancellationToken.None);
            _sockets.TryRemove(socketId, out _);
        }
    }

    private async Task HandleWebSocketAsync(string socketId, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await BroadcastMessageAsync(message);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }
        }
    }

    private async Task BroadcastMessageAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var arraySegment = new ArraySegment<byte>(buffer);

        foreach (var socket in _sockets)
        {
            if (socket.Value.State == WebSocketState.Open)
            {
                await socket.Value.SendAsync(
                    arraySegment, 
                    WebSocketMessageType.Text, 
                    true, 
                    CancellationToken.None);
            }
        }
    }
}