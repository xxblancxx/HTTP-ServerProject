using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserverproject
{
    public class TcpServer
    {
        private static readonly string _rootCatalog = "c:/temp";

        public void ConnectAndStart()
        {


            #region Establish TCP connection

            // Establish port on which server listens for requests
            TcpListener serverSocket = new TcpListener(8080);
            // Using obsolete version - reflected upon this. Easier at our skill level.

            // Starting server
            serverSocket.Start();


            // Establish connection on incoming request. (AcceptTcpClient)
            TcpClient connectionSocket = serverSocket.AcceptTcpClient();

            // Get streams. Then split into specialized objects Reader/writer
            var ns = connectionSocket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;
            #endregion

            try
            {
                // Write Static Content in response.
                string msg = "Hello World!";
                Console.WriteLine(msg); // added for console-UI
                sw.WriteLine(msg);

                // Now a dynamic response -

                // split up the request by spaces
                // split needs an array of string.
                var msg2 = sr.ReadLine().Split(' ');

                //For each string in the array.

                foreach (var s in msg2)
                {
                    Console.WriteLine(s);
                    sw.WriteLine(s);
                    int i = 0;
                    //introduced var int i - because only second string from split, contains desired path.
                    if (i == 1)
                    {
                        //Create stream for specific file. path from TCP-request.
                        FileStream fs = new FileStream(_rootCatalog + s, FileMode.Open, FileAccess.Read);
                        //using Filestream, read content and print it.
                        using (fs)
                        {
                            //Specialized reader
                            StreamReader sr2 = new StreamReader(fs);
                            //Read all content (to end)
                            msg = sr2.ReadToEnd();
                            //print it in console and browser.
                            sw.WriteLine(msg);
                            Console.WriteLine(msg);
                        }
                        i++;
                    }
                    i++;
                }

                // Close connection to actually see response in browser window.
                serverSocket.Stop();
                connectionSocket.Close();
                ns.Close();
                sr.Close();
                sw.Close();
                Console.ReadKey(true);

            }
            catch (NullReferenceException)
            {

                Console.WriteLine("NullReferenceException");

            }






        }
    }
}
