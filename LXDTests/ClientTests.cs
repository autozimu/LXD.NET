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

        [TestMethod]
        public void Certificates_ShouldBeCollection()
        {
            Assert.IsTrue(client.Certificates.Count >= 0);
        }

        [TestMethod]
        public void Containers_ShouldBeCollection()
        {
            Assert.IsTrue(client.Containers.Count >= 0);
        }

        [TestMethod]
        public void Images_ShouldBeCollection()
        {
            Assert.IsTrue(client.Images.Count >= 0);
        }

        [TestMethod]
        public void Networks_ShouldBeCollection()
        {
            Assert.IsTrue(client.Networks.Count >= 0);
        }

        // Disable for now.
        // Right now, `/operations` API returns `{}` when there is no operations in queue,
        // which should actually be `[]`.
        [TestMethod]
        [Ignore]
        public void Operations_ShouldBeCollection()
        {
            Assert.IsTrue(client.Operations.Count >= 0);
        }

        [TestMethod]
        public void Profiles_ShouldBeCollection()
        {
            Assert.IsTrue(client.Profiles.Count >= 0);
        }
    }
}
