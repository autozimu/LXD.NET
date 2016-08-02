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
        public Dictionary<string, Dictionary<string, string>> Devices;
    }
}
