using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public class APIException : ApplicationException
    {
        public IRestResponse Response { get; private set; }
        public string Error { get; private set; }
        public int ErrorCode { get; private set; }
        public JToken Metadata { get; private set; }

        public APIException(IRestResponse response)
        {
            Response = response;

            JToken responseJToken = JToken.Parse(response.Content);
            Error = responseJToken.Value<string>("error");
            ErrorCode = responseJToken.Value<int>("error_code");
            Metadata = responseJToken.SelectToken("metadata");
        }
    }
}
