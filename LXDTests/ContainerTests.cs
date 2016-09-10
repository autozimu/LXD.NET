using LXD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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
            IEnumerable<string> outputs = client.Containers[0].Exec(new[] { "uname" });

            CollectionAssert.AreEqual(
                new[] { "Linux\r\n" },
                outputs.ToArray());
        }
    }
}
