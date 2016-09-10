using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics.Contracts;

namespace LXD
{
    public class LXDException : ApplicationException
    {
        public IRestResponse Response { get; private set; }

        public LXDException(string message)
            : base(message)
        {
            Contract.Requires(message != null);
        }

        public LXDException(IRestResponse response)
            : base(JToken.Parse(response.Content).Value<string>("error"))
        {
            Contract.Requires(response != null);

            Response = response;
        }
    }
}
