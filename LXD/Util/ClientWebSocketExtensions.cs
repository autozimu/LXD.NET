using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LXD
{
    static class WebSocket
    {
        public static async Task<string> ReadAllLines(string url)
        {
			const int WebSocketChunkSize = 1024;

			StringBuilder stdout = new StringBuilder();

			using (ClientWebSocket ws = new ClientWebSocket())
			{
				await ws.ConnectAsync(new Uri(url), CancellationToken.None);

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
						stdout.Append(partialMsg);
					}
				} while (ws.State == WebSocketState.Open);
			}

			return stdout.ToString();
        }
    }
}
