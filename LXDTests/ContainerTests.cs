using LXD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace LXDTests
{
    [TestClass]
    public class ContainerTests
    {
        public TestContext TestContext;
        static Client client;


        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            client = new Client("https://ubuntu:8443", "cert/client.p12", "");
        }

        [TestMethod]
        public void Container_ExecSimpleCommand()
        {
            IEnumerable<ClientWebSocket> wss = client.Containers[0].Exec(new[] { "uname" });
            string stdouterr = Task.Run(() => wss.First().ReadAllLines()).Result;

            Assert.AreEqual("Linux\r\n", stdouterr);
        }

        [TestMethod]
        public void Container_ExecNonInteractiveCommand()
        {
            IEnumerable<ClientWebSocket> wss = client.Containers[0].Exec(new[] { "uname" }, interactive: false);
            string stdout = Task.Run(() => wss.Skip(1).First().ReadAllLines()).Result;

            Assert.AreEqual("Linux\r\n", stdout);
        }
    }
}
