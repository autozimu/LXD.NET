using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LXD
{
    public class JsonSerializer : Newtonsoft.Json.JsonSerializer
    {
        public JsonSerializer()
            : base()
        {
            MissingMemberHandling = MissingMemberHandling.Ignore;
            NullValueHandling = NullValueHandling.Include;
            DefaultValueHandling = DefaultValueHandling.Include;
            ContractResolver = new PascalCasePropertyNamesContractResolver();
        }
    }
}
