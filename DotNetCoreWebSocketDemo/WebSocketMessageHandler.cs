using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreWebSocketDemo
{
    public class WebSocketMessageHandler : SocketsHandler
    {
        public WebSocketMessageHandler(SocketsManager sockets) : base(sockets)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            var socketId = _sockets.GetId(socket);
            // await SendMessageToAll($"{socketId}已加入");
            await SendMessage(socketId, $"{socketId}已加入");
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            await base.OnDisconnected(socket);
            var socketId = _sockets.GetId(socket);
            // await SendMessageToAll($"{socketId}离开了");
            await SendMessage(socketId, $"{socketId}离开了");
        }

        public override async Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = _sockets.GetId(socket);
            var message = $"{socketId} 发送了消息：{Encoding.UTF8.GetString(buffer, 0, result.Count)}";
            // await SendMessageToAll(message);
            await SendMessage(socketId, message);
        }
    }
}
