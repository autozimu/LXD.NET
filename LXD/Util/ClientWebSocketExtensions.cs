using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LXD
{
    public static class ClientWebSocketExtensions
    {
        public static async Task<ClientWebSocket> CreateAndConnect(string url)
        {
            ClientWebSocket ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri(url), CancellationToken.None);
            return ws;
        }

        public static async Task<string> ReadAllLines(this ClientWebSocket ws)
        {
			StringBuilder sb = new StringBuilder();

			const int WebSocketChunkSize = 1024;
            byte[] buffer = new byte[WebSocketChunkSize];
            WebSocketReceiveResult result;
            do
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.EndOfMessage && result.Count == 0)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing WebSocket", CancellationToken.None);
                }
                else
                {
                    string partialMsg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    sb.Append(partialMsg);
                }
            } while (ws.State == WebSocketState.Open);

            return sb.ToString();
        }
    }
}
