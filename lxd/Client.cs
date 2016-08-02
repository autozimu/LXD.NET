using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    public class Client
    {
        public RestClient restClient;
        public X509Certificate2 Cert { get; private set; }
        public bool Verify { get; private set; }

        public Client(string apiEndpoint, X509Certificate2 cert, bool verify = false)
        {
            // Bypass handshake error. LXD do not support TLS 1.3, while this is the default by .NET.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            Verify = verify;
            if (Verify == false)
            {
                // Bypass self-signed certificate error.
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyError) => true;
            }

            restClient = new RestClient($"https://{apiEndpoint}");

            // Add client certificate.
            Cert = cert;
            restClient.ClientCertificates = new X509CertificateCollection();
            restClient.ClientCertificates.Add(Cert);
        }

        void AssertResponse(IRestResponse response, IEnumerable<HttpStatusCode> allowedStatusCodes = null)
        {
            if (response.ErrorException != null)
            {
                throw new LXDAPIException(response.ErrorMessage, null, response.ErrorException);
            }

            allowedStatusCodes = allowedStatusCodes ?? new[] { HttpStatusCode.OK };

            if (!allowedStatusCodes.Contains(response.StatusCode))
            {
                JToken responseJToken = JToken.Parse(response.Content);

                throw new LXDAPIException()
            }
        }
    }
}
