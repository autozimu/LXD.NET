using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public class LXDException : ApplicationException
    {
        public IRestResponse Response { get; private set; }

        public LXDException(string message)
            : base(message)
        {
        }

        public LXDException(IRestResponse response)
            : base(JToken.Parse(response.Content).Value<string>("error"))
        {
            Response = response;
        }
    }
}
