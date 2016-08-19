using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            Contract.Requires(token != null);

            return token.ToObject<T>(new JsonSerializer());
        }
    }
}
