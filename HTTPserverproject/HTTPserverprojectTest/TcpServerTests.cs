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
            TcpClient client = new TcpClient("localhost", 8080);
            NetworkStream ns = client.GetStream();
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;
            string request = "GET /hello.txt HTTP/1.0";
            sw.WriteLine(request);


            // recieve response
            StreamReader sr = new StreamReader(ns);
            string line = sr.ReadLine();

            // Check that initial response is "Hello World" - we made static response as first assignment.
            Assert.AreEqual("Hello World!", line);
            // Unit test of Dynamic response, split by spaces.
            var requestArray = request.Split(' ');
            // foreach loop to match the split in the server. just by our own sent variable.
            foreach (var s in requestArray)
            {
                line = sr.ReadLine();
                Assert.AreEqual(s, line);
            }

            sr.Close();
            sw.Close();
            ns.Close();
        }

        [TestMethod]
        // testmethod to test for response of non-existing file.
        public void ConnectAndStartFileNotFoundTest()
        {
            // Create client to send request, instead of browser.
            TcpClient client = new TcpClient("localhost", 8080);
            NetworkStream ns = client.GetStream();
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;
            //Send request for file we know isn't there
            string request = "GET /doesntexist.txt HTTP/1.0";
            sw.WriteLine(request);

            try
            {
                // See if the response contains error code 404 - File not found.
                StreamReader sr = new StreamReader(ns);
                string line = sr.ReadToEnd();
                if (line.Contains("404"))
                {
                    // if it does, throw exception
                    throw new FileNotFoundException();
                }
                Assert.Fail();
            }
            catch (FileNotFoundException)
            {


            }

        }

        [TestMethod]
        // Create a .txt file, enter a line of content into it - Then test if the server can find and read it.
        public void ConnectAndStartFileResponseTest()
        {
            // path for file
            string path = "c:/temp/";
            // Filename 
            string filename = "existing.txt";
            //contents of txt file.
            string message = "123";
            FileStream fs = new FileStream(path + filename, FileMode.OpenOrCreate, FileAccess.Write);
            using (fs)
            {
                var sw1 = new StreamWriter(fs);
                sw1.AutoFlush = true;

                sw1.WriteLine(message);
            }
            // Create client to send request, instead of browser.
            TcpClient client = new TcpClient("localhost", 8080);
            NetworkStream ns = client.GetStream();
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;
            //Send request for file we know isn't there
            string request = "GET /existing.txt HTTP/1.0";
            sw.WriteLine(request);

            // recieve response
            StreamReader sr = new StreamReader(ns);
            // First line is static response
            string line = sr.ReadLine();
            //second, third and fourth lines are dynamic response, which is request split in 3.
            line = sr.ReadLine();
            line = sr.ReadLine();
            line = sr.ReadLine();
            // fourth one is content-print. so this is the one we want :)
            line = sr.ReadLine();
            Assert.AreEqual(message, line);
        }
    }
}
