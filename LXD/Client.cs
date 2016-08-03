using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LXD
{
    public class Client
    {
        public const string Version = "1.0";

        public static API API { get; private set; }

        public bool Trusted => API.Get(Version).Value<string>("auth") == "trusted";

        public Collection<Certificate> Certificates { get; private set; }
        public Collection<Container> Containers { get; private set; }
        public Collection<Image> Images { get; private set; }
        public Collection<Network> Networks { get; private set; }
        public Collection<Operation> Operations { get; private set; }
        public Collection<Profile> Profiles { get; private set; }


        public Client(string apiEndpoint, X509Certificate2 clientCertificate, bool verify = false)
        {
            API = new API(apiEndpoint, clientCertificate, verify);

            // Verify connection.
            API.Get($"/{Version}");

            Certificates = new Collection<Certificate>($"/{Version}/certificates");
            Containers = new Collection<Container>($"/{Version}/containers");
            Images = new Collection<Image>($"/{Version}/images");
            Networks = new Collection<Network>($"/{Version}/networks");
            Operations = new Collection<Operation>($"/{Version}/operations");
            Profiles = new Collection<Profile>($"/{Version}/profiles");

            // Task.Run(() => GetEventsAsync());
        }

        public Client(string apiEndpoint, string clientCertificateFilename, string password, bool verify = false)
            : this(apiEndpoint, new X509Certificate2(clientCertificateFilename, password), verify)
        {
        }

        public delegate void NewEventHandler(object sender, EventArgs e);

        public event NewEventHandler NewEvent;

        async Task GetEventsAsync()
        {
            using (ClientWebSocket ws = new ClientWebSocket())
            {
                string url = API.BaseUrl.AbsoluteUri.Replace("http", "ws") + $"{Version}/events";
                await ws.ConnectAsync(new Uri(url), CancellationToken.None);

                while (ws.State == WebSocketState.Open)
                {
                    StringBuilder msg = new StringBuilder();
                    byte[] buffer = new byte[1024];
                    WebSocketReceiveResult receiveResult;
                    do
                    {
                        receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        msg.Append(Encoding.UTF8.GetString(buffer, 0, receiveResult.Count));
                    } while (!receiveResult.EndOfMessage);

                    Console.WriteLine(msg);
                    // NewEvent?.Invoke(wsEvent, msg);
                    NewEvent?.Invoke(ws, EventArgs.Empty);
                }
            }
        }
    }
}
