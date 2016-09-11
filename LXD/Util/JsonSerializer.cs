using Newtonsoft.Json;

namespace LXD
{
    internal class JsonSerializer : Newtonsoft.Json.JsonSerializer
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
