using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public class API : RestClient
    {
        public bool Verify { get; private set; }

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public API(string apiEndpoint, X509Certificate2 clientCertificate, bool verify)
            : base(apiEndpoint)
        {
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

        public override IRestResponse Execute(IRestRequest request)
        {
            logger.Trace($"{request.Method}  {request.Resource}");

            IRestResponse response = base.Execute(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            if (!IsSuccessStatusCode(response))
            {
                throw new LXDException(response);
            }

            return response;
        }

        public T Get<T>(string resource)
        {
            return Get(resource).SelectToken("metadata").ToObject<T>(new Serializer().JsonSerializer);
        }

        public JToken Get(string resource)
        {
            IRestRequest request = new Request(resource);
            IRestResponse response = Execute(request);

            return JToken.Parse(response.Content);
        }

        public void Delete(string resource)
        {
            IRestRequest request = new Request(resource, Method.DELETE);
            IRestResponse response = Execute(request);
        }

        public JToken Post(string resource, object payload)
        {
            IRestRequest request = new Request(resource, Method.POST);
            request.AddJsonBody(payload);
            IRestResponse response = Execute(request);

            return JToken.Parse(response.Content);
        }

        public JToken Put(string resource, object payload)
        {
            IRestRequest request = new Request(resource, Method.PUT);
            request.AddJsonBody(payload);
            IRestResponse response = Execute(request);

            return JToken.Parse(response.Content);
        }

        public void WaitForOperationComplete(string operationUrl, int timeout = 0)
        {
            IRestRequest request = new Request($"{operationUrl}/wait");
            if (timeout != 0)
            {
                request.AddParameter("timeout", timeout);
            }

            IRestResponse response = Execute(request);

            Operation operation = JToken.Parse(response.Content).SelectToken("metadata").ToObject<Operation>(new Serializer().JsonSerializer);

            if (operation.StatusCode >= 400 && operation.StatusCode <= 599)
            {
                throw new LXDException(operation.Err);
            }
        }

        bool IsSuccessStatusCode(IRestResponse response)
        {
            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
        }
    }
}
