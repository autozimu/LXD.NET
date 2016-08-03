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

        public JsonNetSerializer Serializer = new JsonNetSerializer();

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

        public T Get<T>(string resource)
        {
            return Get(resource).SelectToken("metadata").ToObject<T>(Serializer.JsonSerializer);
        }

        public JToken Get(string resource)
        {
            IRestRequest request = new RestRequest(resource);
            request.JsonSerializer = Serializer;
            logger.Trace($"{request.Method}  {request.Resource}");
            IRestResponse response = base.Execute(request);
            AssertResponse(response);

            return JToken.Parse(response.Content);
        }

        public void Delete(string resource)
        {
            IRestRequest request = new RestRequest(resource, Method.DELETE);
            logger.Trace($"{request.Method}  {request.Resource}");
            IRestResponse response = base.Execute(request);
            AssertResponse(response);
        }

        public JToken Post(string resource, object payload)
        {
            IRestRequest request = new RestRequest(resource, Method.POST);
            request.JsonSerializer = Serializer;
            request.AddJsonBody(payload);
            logger.Trace($"{request.Method}  {request.Resource}");
            IRestResponse response = base.Execute(request);
            AssertResponse(response);

            return JToken.Parse(response.Content);
        }

        public JToken Put(string route, object payload)
        {
            IRestRequest request = new RestRequest(route, Method.PUT);
            request.JsonSerializer = Serializer;
            request.AddJsonBody(payload);
            logger.Trace($"{request.Method}  {request.Resource}");
            IRestResponse response = base.Execute(request);
            AssertResponse(response, new[] { HttpStatusCode.Accepted });

            return JToken.Parse(response.Content);
        }

        public void WaitForOperationComplete(string operationUrl, int timeout = 0)
        {
            IRestRequest request = new RestRequest($"{operationUrl}/wait");
            request.JsonSerializer = Serializer;
            if (timeout != 0)
            {
                request.AddParameter("timeout", timeout);
            }

            logger.Trace($"{request.Method}  {request.Resource}");
            IRestResponse response = base.Execute(request);
            AssertResponse(response);

            Operation operation = JToken.Parse(response.Content).SelectToken("metadata").ToObject<Operation>(Serializer.JsonSerializer);

            if (operation.StatusCode >= 400)
            {
                throw new LXDException(operation.Err);
            }
        }

        void AssertResponse(IRestResponse response, IEnumerable<HttpStatusCode> allowedStatusCodes = null)
        {
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            allowedStatusCodes = allowedStatusCodes ?? new[] { HttpStatusCode.OK };

            if (!allowedStatusCodes.Contains(response.StatusCode))
            {
                throw new LXDException(response);
            }
        }
    }
}
