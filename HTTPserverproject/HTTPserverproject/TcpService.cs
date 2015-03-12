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
        private List<string> stringList;
        private string[] msg2;


        public TcpService(TcpClient connectionSocket)
        {
            _connectionSocket = connectionSocket; // Constructor takes tcpclient as argument,
            //to reference client made in Server, taking from incoming request on listener
        }

        private void ReadFile(string filepath) // Opens a file-stream and reads the content of a file. must be plain text.
        {
            FileStream fs = new FileStream(_rootCatalog + filepath, FileMode.Open, FileAccess.Read);
            if (fs.Length == 0) // Check if file is empty
            {
                throw new ArgumentNullException();
            }
            using (fs)
            {
                StreamReader sr2 = new StreamReader(fs); //Specialized reader
                using (sr2)
                {
                    string line;
                    while (!sr2.EndOfStream)
                    {
                        line = sr2.ReadLine();
                        stringList.Add(line); // Add all lines to a list, so we can print when we want (after status and content headers)
                    }
                }
            }
        }
        private void EstablishConnection()
        {
            // Get streams. Then split into specialized objects Reader/writer
            _ns = _connectionSocket.GetStream();
            _sr = new StreamReader(_ns);
            _sw = new StreamWriter(_ns);
            _sw.AutoFlush = true;
        }
        private void GiveStaticResponse() // static response if no file is requested
        {
            _msg = "Hello World!";
            Console.WriteLine(_msg); // added for console-UI
            _sw.WriteLine(_msg);
        }

        private void GiveStatusAndContent(string contentType) // Gives 2 Response-Headers. Status code OK, and Content-type with type.
        {
            _sw.WriteLine("HTTP/1.1 200 OK");
            Console.WriteLine("HTTP/1.1 200 OK");
            _sw.WriteLine("Content-Type: " + contentType);
            Console.WriteLine("Content-Type: " + contentType);
        }
        private void GiveDynamicFileResponse()
        {
            // Now a dynamic response -split up the request by spaces
            // split needs an array of string. only need one line (request line) not the rest.
            msg2 = _sr.ReadLine().Split(' ');
            stringList = new List<string>();
            foreach (var s in msg2)
            {
                // Start by checking if the filepath is nothing (just a "/")
                if (s.Equals("/"))
                {
                    GiveStaticResponse(); // Write Static Content in response
                }

                if (!s.Contains("GET") && !s.Contains("HTTP"))
                {
                    
                    ContentHandler ch = new ContentHandler();
                    string contentType = (ch.GetContentType(s));
                    //Create stream for specific file. path from TCP-request.
                    if (contentType.Equals("text/plain"))
                    {
                        ReadFile(s);
                        GiveStatusAndContent(contentType);
                    }
                    if (contentType.Equals("application/pdf"))
                    {
                        GiveStatusAndContent(contentType);
                    }
                    WriteOutFileAndSplitRequest();

                }
            }
        }
        private void CloseConnection()
        { // Closes connection for client
            _connectionSocket.Close();
            _ns.Close();
            _sr.Close();
            _sw.Close();
        }

        private void WriteOutFileAndSplitRequest()
        {
            _sw.Write("\r\n"); // Necessary to show text-response in browser, as the "headers" after response and status code aren'
            foreach (var line in stringList) // Writes out the content of read file, if any.
            {
                if (stringList.Count != 0)
                {
                    Console.WriteLine(line);
                    _sw.WriteLine(line);
                }
            }
            if (msg2.Count() != 0)
            {
                foreach (var s in msg2)//For each string in the array, which contains the Request-Line split into 3.
                { // This method is only used to show we understand the first assigments.
                    Console.WriteLine(s);
                    _sw.WriteLine(s); // write it out
                }
            }

        }
        public void ConnectAndStart()
        {
            EstablishConnection(); // Opens up Tcp client connection for instance.

            try
            {
                GiveDynamicFileResponse();
                // splits request into 3 - gives response. If any, reads content of requested file
            }
            catch (ArgumentNullException) // In case of empty files.
            {
                Console.WriteLine("HTTP/1.1 204 No Content");
                _sw.WriteLine("HTTP/1.1 204 No Content");
            }
            catch (ArgumentException) // This exception is thrown if the requested file extension cannot be handled.
            {
                Console.WriteLine("HTTP/1.1 400 Bad Request");
                _sw.WriteLine("HTTP/1.1 400 Bad Request");
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
