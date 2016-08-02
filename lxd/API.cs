using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
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

        JsonNetSerializer jsonNetSerializer = new JsonNetSerializer();

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
            return Get(route).ToObject<T>(jsonNetSerializer.JsonSerializer);
        }

        public JToken Get(string route)
        {
            Console.WriteLine(route);

            IRestRequest request = new RestRequest(route);
            request.JsonSerializer = jsonNetSerializer;
            IRestResponse response = base.Execute(request);
            AssertResponse(response);

            return JToken.Parse(response.Content).SelectToken("metadata");
        }

        void AssertResponse(IRestResponse response, IEnumerable<HttpStatusCode> allowedStatusCodes = null)
        {
            if (response.ErrorException != null)
            {
                throw new ConnectionException(response.ErrorMessage, response.ErrorException);
            }

            allowedStatusCodes = allowedStatusCodes ?? new[] { HttpStatusCode.OK };

            if (!allowedStatusCodes.Contains(response.StatusCode))
            {
                throw new APIException(response);
            }
        }

    }
}
