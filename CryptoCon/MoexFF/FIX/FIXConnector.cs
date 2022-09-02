using System;
using System.Net.Sockets;

namespace Moex.FIX
{
    public class FIXConnector
    {
        public FIXConnector(string server, int port)
        {
            this.server = server;
            this.port = port;
            this.tcpClient = null;
        }

        private string server { get; set; }
        private int port { get; set; }

        private TcpClient? tcpClient { get; set; }

        private const int TIMEOUT_IN_SEC = 5;

        public bool TryConnect()
        {
            if ((this.tcpClient == null) || !this.tcpClient.Connected)
            {
                this.tcpClient = new TcpClient();
                this.tcpClient.ReceiveTimeout = FIXConnector.TIMEOUT_IN_SEC * 1000;
                this.tcpClient.SendTimeout = FIXConnector.TIMEOUT_IN_SEC * 1000;
                var connectTask = this.tcpClient.ConnectAsync(this.server, this.port);
                connectTask.Wait(new TimeSpan(0, 0, FIXConnector.TIMEOUT_IN_SEC));
            }
            return this.tcpClient.Connected;
        }
    }
}
