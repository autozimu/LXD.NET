using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public struct Certificate
    {
        [JsonProperty("certificate")]
        public string Content;
        public string Fingerprint;
        public string Type;
    }
}
