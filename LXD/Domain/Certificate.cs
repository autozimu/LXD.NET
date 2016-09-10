using Newtonsoft.Json;

namespace LXD.Domain
{
    public class Certificate : RemoteObject
    {
        [JsonProperty("certificate")]
        public string Content;
        public string Fingerprint;
        public string Type;
    }
}
