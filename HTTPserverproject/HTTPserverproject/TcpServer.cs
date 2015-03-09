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


        public void ConnectAndStart()
        {


            #region Establish TCP connection

            // Establish port on which server listens for requests
            TcpListener serverSocket = new TcpListener(8080);
            // Using obsolete version - reflected upon this. Easier at our skill level.

            // Starting server
            serverSocket.Start();


            // NO While Loop Yet
            // Establish connection on incoming request. (AcceptTcpClient)
            TcpClient connectionSocket = serverSocket.AcceptTcpClient();

            // Get streams. Then split into specialized objects Reader/writer
            var ns = connectionSocket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;

            // Write Static Content in response.
            string msg = "Hello World!";
            Console.WriteLine(msg); // added for console-UI
            sw.WriteLine(msg);
            // NO While Loop Yet

            // Close connection to actually see response in browser window.
            serverSocket.Stop();
            connectionSocket.Close();
            ns.Close();
            sr.Close();
            sw.Close();
            Console.ReadKey(true);





            #endregion
        }
    }
}
