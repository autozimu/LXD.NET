using Microsoft.VisualStudio.TestTools.UnitTesting;
using LXD;

namespace LXDTests
{
    [TestClass]
    public class ClientTests
    {
        static Client client;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            client = new Client("https://ubuntu:8443", "cert/client.p12", "");
        }

        [TestMethod]
        public void Trusted_ShouldBeTrue()
        {
            Assert.AreEqual(true, client.Trusted);
        }

        [TestMethod]
        public void Trusted_ShouldBeFalse_WhenNoClientClientCertificateProvided()
        {
            Client unauthClient = new Client("https://ubuntu:8443");

            Assert.AreEqual(false, unauthClient.Trusted);
        }
    }
}
