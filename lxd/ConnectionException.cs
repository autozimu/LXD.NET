using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public class ConnectionException : ApplicationException
    {
        public ConnectionException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}
