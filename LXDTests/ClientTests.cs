using Microsoft.VisualStudio.TestTools.UnitTesting;
using LXD;
using LXD.Domain;

namespace LXDTests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void Constructor()
        {
            Client client = new Client("https://ubuntu:8443", "cert/client.p12", "");

            Assert.AreEqual(true, client.Trusted);
        }
    }
}
