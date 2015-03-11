using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HTTPserverprojectTest
{
    [TestClass]
    public class TcpServiceTests
    {
        private NetworkStream _ns;
        private StreamReader _sr;
        private StreamWriter _sw;
        private TcpClient _client;
        private string _request;

        private void EstablishConnection(string request)
        {
            _client = new TcpClient("localhost", 8080);
            _ns = _client.GetStream();
            _sw = new StreamWriter(_ns);
            _sw.AutoFlush = true;
            _request = request;
            _sw.WriteLine(_request);
            _sr = new StreamReader(_ns);
        }

        private void CloseConnection()
        {
            _sr.Close();
            _sw.Close();
            _ns.Close();
        }
        [TestMethod]
        public void ConnectAndStartResponseTest()
        {
            EstablishConnection("GET /hello.txt HTTP/1.0");


            // recieve response
            _sr = new StreamReader(_ns);
            string line = _sr.ReadLine();

            // Check that initial response is "Hello World" - we made static response as first assignment.
            Assert.AreEqual("Hello World!", line);
            // Unit test of Dynamic response, split by spaces.
            var requestArray = _request.Split(' ');
            // foreach loop to match the split in the server. just by our own sent variable.
            foreach (var s in requestArray)
            {
                line = _sr.ReadLine();
                Assert.AreEqual(s, line);
            }

            CloseConnection();
        }

        [TestMethod]
        // testmethod to test for response of non-existing file.
        public void ConnectAndStartFileNotFoundTest()
        {
            EstablishConnection("GET /doesntExist.txt HTTP/1.0");

            try
            {
                // See if the response contains error code 404 - File not found.
                StreamReader sr = new StreamReader(_ns);
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
                CloseConnection();
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
            EstablishConnection("GET /existing.txt HTTP/1.0");

            // recieve response
            StreamReader sr = new StreamReader(_ns);
            // First line is static response
            string line = sr.ReadLine();
            //second, third and fourth lines are dynamic response, which is request split in 3.
            line = sr.ReadLine();
            line = sr.ReadLine();
            line = sr.ReadLine();
            // fourth one is content-print. so this is the one we want :)
            line = sr.ReadLine();
            CloseConnection();
            Assert.AreEqual(message, line);

        }
    }
}
