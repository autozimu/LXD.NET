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

namespace LXD
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

        public new JToken Execute(IRestRequest request)
        {
            logger.Trace($"{request.Method}  {request.Resource}");

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

            return responseJToken.SelectToken("metadata");
        }

        public JToken ExecuteAndWait(IRestRequest request, int timeout = 0)
        {
            JToken response = Execute(request);

            string type = response.Value<string>("type");
            if (type == "async")
            {
                string operationURL = response.Value<string>("operation");
                return WaitForOperationComplete(operationURL);
            }
            else
            {
                return response;
            }
        }

        public JToken WaitForOperationComplete(string operationUrl, int timeout = 0)
        {
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
            return Execute(new Request(resource)).ToObjectLXDSerialzier<T>();
        }

        public JToken Delete(string resource)
        {
            return ExecuteAndWait(new Request(resource, Method.DELETE));
        }

        public JToken Post(string resource, object payload)
        {
            IRestRequest request = new Request(resource, Method.POST);
            request.AddJsonBody(payload);
            return ExecuteAndWait(request);
        }

        public JToken Put(string resource, object payload)
        {
            IRestRequest request = new Request(resource, Method.PUT);
            request.AddJsonBody(payload);
            return ExecuteAndWait(request);
        }

        bool IsSuccessStatusCode(IRestResponse response)
        {
            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299;
        }
    }

    public static class JTokenExtensions
    {
        public static T ToObjectLXDSerialzier<T>(this JToken token)
        {
            return token.ToObject<T>(new JsonSerializer());
        }
    }
}
