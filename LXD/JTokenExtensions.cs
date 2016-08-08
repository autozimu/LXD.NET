using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LXD
{
    public static class JTokenExtensions
    {
        public static T ToObjectLXDSerialzier<T>(this JToken token)
        {
            return token.ToObject<T>(new JsonSerializer());
        }
    }
}
