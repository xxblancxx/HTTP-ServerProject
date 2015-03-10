using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HTTPserverprojectTest
{
    [TestClass]
    public class TcpServerTests
    {
        [TestMethod]
        public void ConnectAndStartTest()
        {
            TcpClient client = new TcpClient("localhost",8080);
            NetworkStream ns = client.GetStream();
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;
            string request = "GET /hello.txt HTTP/1.0";
            sw.WriteLine(request);
            

            // recieve response
            StreamReader sr = new StreamReader(ns);
            string line = sr.ReadLine();
            
            // Check that initial response is "Hello World" - we made static response as first assignment.
            Assert.AreEqual("Hello World!",line);
            // Unit test of Dynamic response, split by spaces.
            var requestArray = request.Split(' ');
            // foreach loop to match the split in the server. just by our own sent variable.
            foreach (var s in requestArray)
            {
                line = sr.ReadLine();
                Assert.AreEqual(s,line);
            }
            sr.Close();
            sw.Close();
            ns.Close();
        }
    }
}
