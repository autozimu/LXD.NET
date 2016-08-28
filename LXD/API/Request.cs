using RestSharp;

namespace LXD
{
    public class Request : RestRequest
    {
        public Request(string resouce)
            : base(resouce)
        {
            JsonSerializer = new RestSharpSerializer();
        }

        public Request(string resouce, Method method)
            : base(resouce, method)
        {
            JsonSerializer = new RestSharpSerializer();
        }
    }
}
