using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserverproject
{
    public class Program
    {

        public static void Main(string[] args)
        {
            while (true)
            {
                TcpServer server = new TcpServer();
                server.ChatConnection();
            }
        }


    }
}
