using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static LXD.Profile;

namespace LXD
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

        public void Start(int timeout = 30, bool stateful = false)
        {
            ContainerStatePut payload = new ContainerStatePut()
            {
                Action = "start",
                Timeout = timeout,
                Stateful = stateful,
            };

            string operationUrl = Client.API.Put($"/{Client.Version}/containers/{Name}/state", payload).Value<string>("operation");

            Client.API.WaitForOperationComplete(operationUrl);
        }

        public void Stop(int timeout = 30, bool force = false, bool stateful = false)
        {
            ContainerStatePut payload = new ContainerStatePut()
            {
                Action = "stop",
                Timeout = timeout,
                Force = force,
                Stateful = stateful,
            };

            string operatioUrl = Client.API.Put($"/{Client.Version}/containers/{Name}/state", payload).Value<string>("operation");

            Client.API.WaitForOperationComplete(operatioUrl);
        }


        public void Restart(int timeout = 30, bool force = false)
        {
            ContainerStatePut payload = new ContainerStatePut()
            {
                Action = "restart",
                Timeout = timeout,
                Force = force,
            };

            string operationUrl = Client.API.Put($"/{Client.Version}/container/{Name}/state", payload).Value<string>("operation");

            Client.API.WaitForOperationComplete(operationUrl);
        }

        public void Freeze(int timeout = 30)
        {
            ContainerStatePut payload = new ContainerStatePut()
            {
                Action = "freeze",
                Timeout = timeout,
            };

            string operationUrl = Client.API.Put($"/{Client.Version}/container/{Name}/state", payload).Value<string>("operation");

            Client.API.WaitForOperationComplete(operationUrl);
        }

        public void Unfreeze(int timeout = 30)
        {
            ContainerStatePut payload = new ContainerStatePut()
            {
                Action = "unfreeze",
                Timeout = timeout,
            };

            string operationUrl = Client.API.Put($"/{Client.Version}/container/{Name}/state", payload).Value<string>("operation");

            Client.API.WaitForOperationComplete(operationUrl);
        }

        public struct ContainerStatePut
        {
            public string Action;
            public int Timeout;
            public bool Force;
            public bool Stateful;
        }

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

            Client.API.Post($"/{Client.Version}/containers/{Name}/exec", task);

            // TODO: return websockets.
            return null;
        }

        public struct ContainerExec
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
            IRestRequest request = new RestRequest($"/{Client.Version}/containers/{Name}/files");
            request.AddParameter("path", path);
            IRestResponse response = Client.API.Execute(request);
            return response.Content;
        }

        // TODO: fill body.
        public void PutFile(string path, byte[] content)
        {
            return;
        }

        public ContainerState State => Client.API.Get<ContainerState>($"/{Client.Version}/containers/{Name}/state");
        public Collection<object> Logs => new Collection<object>($"/{Client.Version}/containers/{Name}/logs");
        public Collection<Container> Snapshots => new Collection<Container>($"/{Client.Version}/containers/{Name}/snapshots");
    }
}
