using System;
using System.Net;
using System.Net.Sockets;

namespace Moex.FAST
{
    public class FASTConnector
    {
        public FASTConnector(string srcIP, string ip, int port)
        {
            this.srcIP = srcIP;
            this.ip = ip;
            this.port = port;
        }

        public FASTConnector(Config.UDPConnector connector)
        {
            this.srcIP = connector.SrcIP;
            this.ip = connector.IP;
            this.port = connector.Port;
        }

        private string srcIP { get; set; }

        private string ip { get; set; }
        private int port { get; set; }

        private UdpClient? udpClient { get; set; }

        public bool TryConnect()
        {
            if (this.udpClient == null)
            {
                this.udpClient = new UdpClient(this.port);

                var multicastAddress = IPAddress.Parse(this.ip);
                var sourceAddress = IPAddress.Parse(this.srcIP);
                if (multicastAddress == null)
                {
                    Console.WriteLine("MulticastAddress null.");
                }
                else if (sourceAddress == null)
                {
                    Console.WriteLine("SourceAddress null.");
                }
                else
                {
                    var localAddress = IPAddress.Any;
                    byte[] membershipAddresses = new byte[12];
                    Buffer.BlockCopy(multicastAddress.GetAddressBytes(), 0, membershipAddresses, 0, 4);
                    Buffer.BlockCopy(sourceAddress.GetAddressBytes(), 0, membershipAddresses, 4, 4);
                    Buffer.BlockCopy(localAddress.GetAddressBytes(), 0, membershipAddresses, 8, 4);
                    this.udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddSourceMembership, membershipAddresses);
                }

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] result = this.udpClient.Receive(ref endPoint);
                Console.WriteLine($"Receive \"{result.Length}\" bytes...");
                int seqNum = (int)result[0] + ((int)result[1] << 8) + ((int)result[2] << 16) + ((int)result[3] << 24);
                Console.WriteLine($"SeqNum {seqNum}...");

                this.udpClient.Close();
            }
            return true;
        }
    }
}
