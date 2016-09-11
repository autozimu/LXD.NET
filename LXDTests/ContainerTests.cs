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
        private static Client s_client;


        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            s_client = new Client("https://ubuntu:8443", "cert/client.p12", "");
        }

        [TestMethod]
        public void Container_ExecSimpleCommand()
        {
            ClientWebSocket ws = s_client.Containers[0].Exec(new[] { "uname" }).First();
            string stdouterr = Task.Run(() => ws.ReadLinesAsync()).Result;

            Assert.AreEqual("Linux\r\n", stdouterr);
        }

        [TestMethod]
        public void Container_ExecNonInteractiveCommand()
        {
            IEnumerable<ClientWebSocket> wss = s_client.Containers[0].Exec(new[] { "uname" }, interactive: false);
            string stdout = Task.Run(() => wss.Skip(1).First().ReadLinesAsync()).Result;

            Assert.AreEqual("Linux\n", stdout);
        }

        [TestMethod]
        [Ignore]
        public void Container_ExecCommandWithInput()
        {
            ClientWebSocket ws = s_client.Containers[0].Exec(new[] { "cat" }).First();

            Task.Run(() => ws.WriteAsync("Yo\r\n")).Wait();
            string output = Task.Run(() => ws.ReadLinesAsync()).Result;

            Assert.AreEqual("Yo\r\n", output);
        }
    }
}
