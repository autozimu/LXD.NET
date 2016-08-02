using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<string, Device> Devices;
        public bool Ephemaral;
        public Dictionary<string, string> ExpandedConfig;
        public Dictionary<string, Device> ExpandedDevices;
        public string Name;
        public string[] Profiles;
        public bool Stateful;
        public string Status;
        public int StatusCode;
    }
}
