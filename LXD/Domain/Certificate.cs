using Newtonsoft.Json;

namespace LXD.Domain
{
    public struct Certificate
    {
        [JsonProperty("certificate")]
        public string Content;
        public string Fingerprint;
        public string Type;
    }
}
