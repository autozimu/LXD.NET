using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public struct Profile
    {
        public string Name;
        public Dictionary<string, string> Config;
        public string Description;
        public Dictionary<string, Device> Devices;

        public struct Device 
        {
            public string Path;
            public string Source;
            public string Type;
        }

        // TODO
        public struct ExpandedDevice
        {
            public string Name;
            public string NicType;
            public string Parent;
            public string Type;
            public string Path;
        }
    }
}
