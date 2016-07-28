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
    class Program
    {
        static void Main(string[] args)
        {
			// Bypass handshake error. C# default tls13 is not supported by lxd.
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
			// Bypass self-signed certificate error.
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyError) => true;
            // Bypass slow proxy auto configuration.
            WebRequest.DefaultWebProxy = null;

            RestClient client = new RestClient("https://ubuntu:8443");

			// Add client certificate.
			client.ClientCertificates = new X509CertificateCollection();
			client.ClientCertificates.Add(new X509Certificate2("cert/client.p12"));

			RestRequest request = new RestRequest("/1.0");

            IRestResponse response = client.Execute(request);

            Contract.Assume(response != null);
            if (response.ErrorException != null)
                throw response.ErrorException;

            string content = response.Content;

            Console.WriteLine(content);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
