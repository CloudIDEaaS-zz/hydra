using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Utils
{
    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> sockets = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetSocketById(string id)
        {
            return sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return sockets;
        }

        public string GetId(WebSocket socket)
        {
            return sockets.FirstOrDefault(p => p.Value == socket).Key;
        }
        public void AddSocket(WebSocket socket)
        {
            sockets.TryAdd(CreateConnectionId(), socket);
        }

        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            sockets.TryRemove(id, out socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, statusDescription: "Closed by the ConnectionManager", cancellationToken: CancellationToken.None);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
