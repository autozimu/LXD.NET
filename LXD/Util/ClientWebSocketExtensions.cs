using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LXD
{
    public static class ClientWebSocketExtensions
    {
        public static async Task<ClientWebSocket> CreateAndConnectAsync(string url)
        {
            ClientWebSocket ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri(url), CancellationToken.None);
            return ws;
        }

        public static async Task<string> ReadLinesAsync(this ClientWebSocket ws)
        {
            StringBuilder sb = new StringBuilder();

            const int WebSocketChunkSize = 1024;
            byte[] buffer = new byte[WebSocketChunkSize];
            WebSocketReceiveResult result;
            do
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string partialMsg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                sb.Append(partialMsg);
            } while (!result.EndOfMessage && ws.State == WebSocketState.Open);

            return sb.ToString();
        }

        public static async Task WriteAsync(this ClientWebSocket ws, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await ws.SendAsync(
                new ArraySegment<byte>(buffer), 
                WebSocketMessageType.Text, 
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
        }
    }
}
