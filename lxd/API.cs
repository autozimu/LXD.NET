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

        public T Get<T>(string route)
        {
            return Get(route).SelectToken("metadata").ToObject<T>(Serializer.JsonSerializer);
        }

        public JToken Get(string route)
        {
            logger.Trace($"{Method.GET}  {route}");

            IRestRequest request = new RestRequest(route);
            request.JsonSerializer = Serializer;
            IRestResponse response = base.Execute(request);
            AssertResponse(response);

            return JToken.Parse(response.Content);
        }

        public void Delete(string route)
        {
            logger.Trace($"{Method.DELETE}  {route}");

            IRestRequest request = new RestRequest(route, Method.DELETE);
            IRestResponse response = base.Execute(request);
            AssertResponse(response);
        }

        public JToken Post(string route, object payload)
        {
            logger.Trace($"{Method.POST}  {route}");

            IRestRequest request = new RestRequest(route, Method.POST);
            request.JsonSerializer = Serializer;
            request.AddJsonBody(payload);
            IRestResponse response = base.Execute(request);
            AssertResponse(response);

            return JToken.Parse(response.Content);
        }

        public JToken Put(string route, object payload)
        {
            logger.Trace($"{Method.PUT}  {route}");

            IRestRequest request = new RestRequest(route, Method.PUT);
            request.JsonSerializer = Serializer;
            request.AddJsonBody(payload);
            IRestResponse response = base.Execute(request);
            AssertResponse(response, new[] { HttpStatusCode.Accepted });

            return JToken.Parse(response.Content);
        }

        public void WaitForOperationComplete(string operationUrl, int timeout = 0)
        {
            logger.Trace($"{Method.GET} {operationUrl}");

            IRestRequest request = new RestRequest($"{operationUrl}/wait");
            request.JsonSerializer = Serializer;
            if (timeout != 0)
            {
                request.AddParameter("timeout", timeout);
            }

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
