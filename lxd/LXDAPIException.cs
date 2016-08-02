using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public class LXDAPIException : ApplicationException
    {
        public IRestResponse Response { get; private set; }

        public LXDAPIException(string message, IRestResponse reponse, Exception ex)
            : base(message, ex)
        {
            Response = reponse;
        }
    }
}
