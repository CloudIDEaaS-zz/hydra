using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utils
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate next;
        private WebSocketHandler webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, IServiceProvider serviceProvider, Func<IServiceProvider, WebSocketHandler> getWebSocketHandler)
        {
            this.next = next;
            this.webSocketHandler = getWebSocketHandler(serviceProvider);
        }

        public async Task Invoke(HttpContext context)
        {
            WebSocket socket;

            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            socket = await context.WebSockets.AcceptWebSocketAsync();
            await webSocketHandler.OnConnected(socket);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await webSocketHandler.ReceiveAsync(socket, result, buffer);
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocketHandler.OnDisconnected(socket);
                    return;
                }

            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
