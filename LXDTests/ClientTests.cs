using Microsoft.VisualStudio.TestTools.UnitTesting;
using LXD;

namespace LXDTests
{
    [TestClass]
    public class ClientTests
    {
        private static Client s_client;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            s_client = new Client("https://ubuntu:8443", "cert/client.p12", "");
        }

        [TestMethod]
        public void Trusted_ShouldBeTrue()
        {
            Assert.AreEqual(true, s_client.Trusted);
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
            Assert.IsTrue(s_client.Certificates.Count >= 0);
        }

        [TestMethod]
        public void Containers_ShouldBeCollection()
        {
            Assert.IsTrue(s_client.Containers.Count >= 0);
        }

        [TestMethod]
        public void Images_ShouldBeCollection()
        {
            Assert.IsTrue(s_client.Images.Count >= 0);
        }

        [TestMethod]
        public void Networks_ShouldBeCollection()
        {
            Assert.IsTrue(s_client.Networks.Count >= 0);
        }

        // Disable for now.
        // Right now, `/operations` API returns `{}` when there is no operations in queue,
        // which should actually be `[]`.
        [TestMethod]
        [Ignore]
        public void Operations_ShouldBeCollection()
        {
            Assert.IsTrue(s_client.Operations.Count >= 0);
        }

        [TestMethod]
        public void Profiles_ShouldBeCollection()
        {
            Assert.IsTrue(s_client.Profiles.Count >= 0);
        }
    }
}
