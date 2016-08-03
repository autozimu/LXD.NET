using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public class Request : RestRequest
    {
        public Request(string resouce)
            : base(resouce)
        {
            JsonSerializer = new Serializer();
        }

        public Request(string resouce, Method method)
            : base(resouce, method)
        {
            JsonSerializer = new Serializer();
        }
    }
}
