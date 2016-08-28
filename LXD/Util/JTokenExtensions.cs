using System.Diagnostics.Contracts;
using Newtonsoft.Json.Linq;

namespace LXD
{
    public static class JTokenExtensions
    {
        public static T ToObjectLXDSerialzier<T>(this JToken token)
        {
            Contract.Requires(token != null);

            return token.ToObject<T>(new JsonSerializer());
        }
    }
}
