using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading;
using System.Net.WebSockets;

namespace lxd
{
	class ContainerExecDTO
	{
		public string[] command = "cat /etc/issue".Split(' ');
		[JsonProperty("wait-for-websocket")]
		public bool waitForWebsocket = true;
		public bool interactive = true;
	}

    class Program
    {
		const string serviceAddr = "ubuntu:8443";

        static void Main(string[] args)
        {
			// Bypass handshake error. LXD do not support TLS 1.3, while this is the default by .NET.
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
			// Bypass self-signed certificate error.
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyError) => true;
            // Bypass slow proxy auto configuration.
            WebRequest.DefaultWebProxy = null;

            RestClient client = new RestClient($"https://{serviceAddr}");

			// Add client certificate.
			client.ClientCertificates = new X509CertificateCollection();
			client.ClientCertificates.Add(new X509Certificate2("cert/client.p12"));

			// Exec
			RestRequest request = new RestRequest("/1.0/containers/alpine/exec", Method.POST);
			request.JsonSerializer = new JsonNetSerializer();
			request.AddJsonBody(new ContainerExecDTO());
			IRestResponse response = client.Execute(request);

			Contract.Assume(response != null);
			if (response.ErrorException != null)
			{
				throw response.ErrorException;
			}

			string operationUrl = JToken.Parse(response.Content).Value<string>("operation");

			// Get fds secret.
			request = new RestRequest(operationUrl);
			response = client.Execute(request);

			if (response.ErrorException != null)
			{
				throw response.ErrorException;
			}

			string secret = JToken.Parse(response.Content).SelectToken("metadata.metadata.fds.0").Value<string>();

			// Connect websocket.
			string wsUrl = $"wss://{serviceAddr}/{operationUrl}/websocket?secret={secret}";
			//string wsUrl = "wss://echo.websocket.org";

			string stdout = ReadAllLinesFromWebSocketAsync(wsUrl).Result;

			Console.WriteLine(stdout);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

		static async Task<string> ReadAllLinesFromWebSocketAsync(string url)
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

					string partialMsg = Encoding.UTF8.GetString(buffer, 0, result.Count);
					stdout.Append(partialMsg);
				} while (!result.EndOfMessage);
			}

			return stdout.ToString();
		}
    }
}
