using System;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
        string _testFilePath;
        string _testFileContent;

        private void EstablishConnection(string request)
        {
            // This methods creates a client and streams to read and write.
            // The reason for the client is so that the test sends requests to test the servers handling.
            _client = new TcpClient("localhost", 8080);
            _ns = _client.GetStream();
            _sw = new StreamWriter(_ns);
            _sw.AutoFlush = true;
            //Request as a method argument so each test method can have different requests.
            _request = request;
            _sw.WriteLine(_request);
            _sr = new StreamReader(_ns);
            
        }

        public void CreateTestFile()
        {
            _testFilePath = "c:/temp/existing.txt";
            FileStream fs = new FileStream(_testFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            using (fs)
            {
                var sw1 = new StreamWriter(fs);
                sw1.AutoFlush = true;
                _testFileContent = "123";
                sw1.Write(_testFileContent);
            }
        }


        private void CloseConnection()
        {
            _sr.Close();
            _sw.Close();
            _ns.Close();
        }

        [TestMethod]
        public void ConnectAndStartDynamicResponseTest()
        {
            CreateTestFile();
            EstablishConnection("GET /existing.txt HTTP/1.1");


            // recieve response
            _sr = new StreamReader(_ns);
            string line = _sr.ReadLine();

            // Initial Response should be status saying OK
            Assert.AreEqual("HTTP/1.1 200 OK", line);
            // Second response should be content of read file.
            line = _sr.ReadLine();
            Assert.AreEqual(_testFileContent,line);

            var requestArray = _request.Split(' '); // we split up request by spaces
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
                string line = sr.ReadLine();
                if (line.Equals("HTTP/1.1 404 Not Found"))
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
        public void ConnectAndStartStaticResponseTest() // test for static response
        {
            EstablishConnection("GET / HTTP/1.1"); // send request without path
            string line = _sr.ReadLine();
            Assert.AreEqual("Hello World!",line); //static response should be "Hello World!"
        }
    }
}
