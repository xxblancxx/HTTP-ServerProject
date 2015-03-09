using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserverproject
{
    public class Service
    {
        private TcpClient _connectionSocket;

        public Service(TcpClient client)
        {
            _connectionSocket = client;
        }

        public void ChatConnection()
        {
            
        }
    }
}
