using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserverproject
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Establish TCP connection

            // Establish port on which server listens for requests
            TcpListener serverSocket = new TcpListener(80);
            // Using obsolete version - reflected upon this. Easier at our skill level.

            // Starting server
            serverSocket.Start();

            // INSERT WHILE LOOP START HERE. Coming later as it is single-threaded in the beginning.
            // Establish connection on incoming request. (AcceptTcpClient)
            TcpClient connectionSocket = serverSocket.AcceptTcpClient();

            // Get streams. Then split into specialized objects Reader/writer
            var ns = connectionSocket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;

            // write out incoming request.
            Console.WriteLine(sr.ReadLine());
            Console.ReadKey(true);

            #endregion


        }
    }
}
