using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCoreWebSocketDemo
{
    /// <summary>
    /// websocket中间件配置
    /// </summary>
    public class SocketsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SocketsHandler _handler;

        public SocketsMiddleware(RequestDelegate next, SocketsHandler handler)
        {
            _next = next;
            _handler = handler;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                // 转换当前连接为一个 ws 连接
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                await _handler.OnConnected(socket);

                // 接收消息的 buffer
                var buffer = new byte[1024 * 4];
                // 判断连接类型，并执行相应操作
                while (socket.State == WebSocketState.Open)
                {
                    // buffer 就是接收到的消息体，可以根据需要进行转换。
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            await _handler.Receive(socket, result, buffer);
                            break;
                        case WebSocketMessageType.Close:
                            await _handler.OnDisconnected(socket);
                            break;
                        case WebSocketMessageType.Binary:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
