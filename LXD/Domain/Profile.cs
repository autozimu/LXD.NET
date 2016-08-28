using System.Collections.Generic;

namespace LXD.Domain
{
    public struct Profile
    {
        public string Name;
        public Dictionary<string, string> Config;
        public string Description;
        public Dictionary<string, Dictionary<string, string>> Devices;
    }
}
