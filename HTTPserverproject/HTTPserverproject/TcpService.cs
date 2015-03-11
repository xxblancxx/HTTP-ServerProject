using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserverproject
{
    public class TcpService
    {
        private static readonly string _rootCatalog = "c:/temp";
        private NetworkStream _ns;
        private StreamReader _sr;
        private StreamWriter _sw;
        private TcpClient _connectionSocket;
        private string _msg;

        public TcpService(TcpClient connectionSocket)
        {
            _connectionSocket = connectionSocket; // Constructor takes tcpclient as argument,
            //to reference client made in Server, taking from incoming request on listener
        }
        private void EstablishConnection()
        {
            // Get streams. Then split into specialized objects Reader/writer
            _ns = _connectionSocket.GetStream();
            _sr = new StreamReader(_ns);
            _sw = new StreamWriter(_ns);
            _sw.AutoFlush = true;
        }
        private void GiveStaticResponse()
        {
            _msg = "Hello World!";
            Console.WriteLine(_msg); // added for console-UI
            _sw.WriteLine(_msg);
        }
        private void GiveDynamicFileResponse()
        {
            // Now a dynamic response -split up the request by spaces
            // split needs an array of string. only need one line (request line) not the rest.
            var msg2 = _sr.ReadLine().Split(' ');

            
            foreach (var s in msg2)
            {
                // Start by checking if the filepath is nothing (just a "/")
                if (s.Equals("/"))
                {
                    GiveStaticResponse(); // Write Static Content in response
                }

                if (!s.Contains("GET") && !s.Contains("HTTP"))
                {
                    //Create stream for specific file. path from TCP-request.
                    FileStream fs = new FileStream(_rootCatalog + s, FileMode.Open, FileAccess.Read);

                    if (fs.Length == 0) // Check if file is empty
                    {
                        throw new ArgumentNullException();
                    }
                    //using Filestream, read content and print it.
                    _sw.WriteLine("HTTP/1.1 200 OK");
                    Console.WriteLine("HTTP/1.1 200 OK");
                    using (fs)
                    {
                        StreamReader sr2 = new StreamReader(fs); //Specialized reader
                        _msg = sr2.ReadToEnd(); //Read all content (to end)
                        _sw.WriteLine(_msg); // Response
                        Console.WriteLine(_msg); // console-print
                    }
                }
            }
            foreach (var s in msg2)//For each string in the array.
            {
                Console.WriteLine(s);
                _sw.WriteLine(s); // write it out
            }
        }
        private void CloseConnection()
        { // Closes connection for client
            _connectionSocket.Close();
            _ns.Close();
            _sr.Close();
            _sw.Close();
        }
        public void ConnectAndStart()
        {
            EstablishConnection(); // Opens up Tcp client connection for instance.

            try
            {
                GiveDynamicFileResponse(); // splits request into 3 - gives response. If any, reads content of requested file
            }
            catch (ArgumentNullException) // In case of empty files.
            {
                Console.WriteLine("HTTP/1.1 204 No Content");
                _sw.WriteLine("HTTP/1.1 204 No Content");
            }
            catch (FileNotFoundException) // If the file requested isn't there.
            {
                Console.WriteLine("HTTP/1.1 404 Not Found");
                _sw.WriteLine("HTTP/1.1 404 Not Found");
            }
            finally
            {
                CloseConnection(); // Close connection
            }
        }


    }
}
