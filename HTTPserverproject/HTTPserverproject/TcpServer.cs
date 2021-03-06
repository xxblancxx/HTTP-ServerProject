﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTTPserverproject
{
    public class TcpServer
    {
        public void ChatConnection()
        {
            // Establish port on which server listens for requests
            TcpListener serverSocket = new TcpListener(8080);
            // Using obsolete version - reflected upon this. Easier at our skill level.

            serverSocket.Start(); // start server
            // Clients are handled in an infinite loop, creating Tasks for each new accepted tcp request
            while (true)
            {
                //wait for incoming request
                TcpClient connectionSocket = serverSocket.AcceptTcpClient();
                // establish connection on accepted request

                TcpService client = new TcpService(connectionSocket);
                // makes an instance of the service, which handles request.

                Task t = new Task(client.ConnectAndStart);
                // Creates an async task to run the Service to handle 1 request
                t.Start(); // start the task
            }
        }
    }
}
