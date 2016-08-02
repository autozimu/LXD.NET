using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static lxd.Profile;

namespace lxd
{
    public struct Container
    {
        public string Architecture;
        public Dictionary<string, string> Config;
        public DateTime CreatedAt;
        public Dictionary<string, Dictionary<string, string>> Devices;
        public bool Ephemaral;
        public Dictionary<string, string> ExpandedConfig;
        public Dictionary<string, Dictionary<string, string>> ExpandedDevices;
        public string Name;
        public string[] Profiles;
        public bool Stateful;
        public string Status;
        public int StatusCode;

        public IEnumerable<ClientWebSocket> Exec(string[] command,
            Dictionary<string, string> environment = null,
            bool waitForWebSocket = false,
            bool interactive = true,
            int width = 80,
            int height = 25
            )
        {
            ContainerExec task = new ContainerExec()
            {
                Command = command,
                Environment = environment,
                WaitForWebSocket = waitForWebSocket,
                Width = width,
                Height = height,
            };

            Client.API.Post($"{Client.Version}/{Name}/exec", task);

            // TODO: return websockets.
            return null;
        }

        struct ContainerExec
        {
            public string[] Command;
            public Dictionary<string, string> Environment;
            [JsonProperty("wait-for-websocket")]
            public bool WaitForWebSocket;
            public bool Interactive;
            public int Width;
            public int Height;
        }

        public string GetFile(string path)
        {
            IRestRequest request = new RestRequest($"{Client.Version}/{Name}/files");
            request.AddParameter("path", path);
            IRestResponse response = Client.API.Execute(request);
            return response.Content;
        }

        // TODO: fill body.
        public void PutFile(string path, byte[] content)
        {
            return;
        }

        public ContainerState State => Client.API.Get<ContainerState>($"{Client.Version}/{Name}/state");
        public Collection<object> Logs => new Collection<object>($"{Client.Version}/{Name}/logs");
        public Collection<Container> Snapshots => new Collection<Container>($"{Client.Version}/{Name}/snapshots");
    }
}
