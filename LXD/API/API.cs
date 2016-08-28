using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Contracts;

namespace LXD
{
    public class API : RestClient
    {
        public bool Verify { get; private set; }

        public API(string baseUrl, X509Certificate2 clientCertificate, bool verify)
            : base(baseUrl)
        {
            Contract.Requires(baseUrl != null);

            // Bypass handshake error. LXD do not support TLS 1.3, while this is the default by .NET.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            Verify = verify;
            if (Verify == false)
            {
                // Bypass self-signed certificate error.
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyError) => true;
            }

            ClientCertificates = new X509CertificateCollection();
            ClientCertificates.Add(clientCertificate);
        }

        public new JToken Execute(IRestRequest request)
        {
            Contract.Requires(request != null);

            IRestResponse response = base.Execute(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            // HTTP status is success.
            if (!IsSuccessStatusCode(response))
            {
                throw new LXDException(response);
            }

            // API status is success.
            JToken responseJToken = JToken.Parse(response.Content);
            int statusCode = responseJToken.Value<int>("status_code");
            if (statusCode >= 400 && statusCode <= 599)
            {
                throw new LXDException(response);
            }

            return responseJToken;
        }

        public JToken WaitForOperationComplete(JToken response, int timeout = 0)
        {
            Contract.Requires(response != null);
            Contract.Assert(response.Value<string>("type") == "async");
            string operationUrl = response.Value<string>("operation");

            IRestRequest request = new Request($"{operationUrl}/wait");
            if (timeout != 0)
            {
                request.AddParameter("timeout", timeout);
            }

            return Execute(request);
        }

        public JToken Get(string resource)
        {
            return Execute(new Request(resource));
        }

        public T Get<T>(string resource)
        {
            return Get(resource).SelectToken("metadata").ToObjectLXDSerialzier<T>();
        }

        public JToken Delete(string resource)
        {
            return Execute(new Request(resource, Method.DELETE));
        }

        public JToken Post(string resource, object payload)
        {
            IRestRequest request = new Request(resource, Method.POST);
            request.AddJsonBody(payload);
            return Execute(request);
        }

        public JToken Put(string resource, object payload)
        {
            IRestRequest request = new Request(resource, Method.PUT);
            request.AddJsonBody(payload);
            return Execute(request);
        }

        bool IsSuccessStatusCode(IRestResponse response)
        {
            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
        }

        public string BaseUrlWebSocket => BaseUrl.AbsoluteUri.Replace("http", "ws");
    }
}
