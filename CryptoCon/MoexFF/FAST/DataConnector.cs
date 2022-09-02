using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Moex.FAST.Statistics;

namespace Moex.FAST
{
    public sealed class DataConnector
    //internal sealed class DataConnector
    {
        public DataConnector(string type, Config.UDPConnector config, bool consoleLog = false)
        {
            this.type = type;
            this.config = config;
            this.consoleLog = consoleLog;
            this.client = null;
            this.messages = new MessageQueue();
            this.currentNumber = 0;
        }

        private string type { get; set; }

        private Config.UDPConnector config { get; set; }

        private bool consoleLog { get; set; }

        private UdpClient? client { get; set; }

        public class MessageInfo
        {
            public MessageInfo()
            {
                this.Number = Int32.MaxValue;
                this.Data = null;
            }

            public int Number { get; private set; }

            public byte[]? Data { get; private set; }

            public void SetData(byte[] data)
            {
                this.Data = data;
                this.Number = BitConverter.ToInt32(this.Data, 0);
            }
        }

        private MessageQueue messages { get; set; }

        private int currentNumber { get; set; }

        // Предполагается что один поток данных не может обогнать другой поток данных более, чем на 10 сообщений...
        public static int MAX_DESYNC = 10;

        public MessageInfo GetFirstMessage(int currentMessage, Statistics.ConnectorStatistics? connectorStatistics)
        {
            connectorStatistics?.SetMessages(this.messages.GetCount());
            var firstMessage = this.messages.Peek();
            if (firstMessage.Number == Int32.MaxValue)
            {
                return firstMessage;
            }
            if (currentMessage == 0)
            {
                return firstMessage;
            }
            if ((firstMessage.Number < DataConnector.MAX_DESYNC) && (currentMessage >= DataConnector.MAX_DESYNC))
            {
                return firstMessage;
            }
            while (firstMessage.Number <= currentMessage)
            {
                this.DeleteMessage(connectorStatistics);
                connectorStatistics?.MessageSkipped();
                firstMessage = this.messages.Peek();
            }
            return firstMessage;
        }

        public void FirstMessageProcessed(Statistics.ConnectorStatistics? connectorStatistics)
        {
            this.DeleteMessage(connectorStatistics);
            connectorStatistics?.MessageProcessed();
        }

        private void DeleteMessage(Statistics.ConnectorStatistics? connectorStatistics)
        {
            MessageInfo messageInfo = this.messages.Dequeue();
            if (((this.currentNumber + 1) == messageInfo.Number) || (this.currentNumber == 0))
            {
                this.currentNumber = messageInfo.Number;
                return;
            }
            else if ((messageInfo.Number < DataConnector.MAX_DESYNC) && (this.currentNumber >= DataConnector.MAX_DESYNC))
            {
                this.currentNumber = 0;
            }
            while ((this.currentNumber + 1) < messageInfo.Number)
            {
                this.currentNumber++;
                connectorStatistics?.MessageLost();
            }
            this.currentNumber = messageInfo.Number;
        }

        public async Task Process(CancellationToken cancellationToken, NLog.Logger? logger)
        {
            this.client = new UdpClient(this.config.Port); // throw

            IPAddress? multicastAddress;
            IPAddress? sourceAddress;
            if (IPAddress.TryParse(this.config.IP, out multicastAddress) == false)
            {
                throw new ArgumentException($"Invalid multicast address - \"{this.config.IP}\"");
            }
            else if (IPAddress.TryParse(this.config.SrcIP, out sourceAddress) == false)
            {
                throw new ArgumentException($"Invalid source address - \"{this.config.SrcIP}\"");
            }
            else
            {
                byte[] membershipAddresses = new byte[12];
                Buffer.BlockCopy(multicastAddress.GetAddressBytes(), 0, membershipAddresses, 0, 4);
                Buffer.BlockCopy(sourceAddress.GetAddressBytes(), 0, membershipAddresses, 4, 4);
                var localAddress = IPAddress.Any;
                Buffer.BlockCopy(localAddress.GetAddressBytes(), 0, membershipAddresses, 8, 4);
                this.client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddSourceMembership, membershipAddresses);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await this.client.ReceiveAsync(cancellationToken);
                    if (result.Buffer != null)
                    {
                        if (this.consoleLog)
                        {
                            Console.WriteLine($"{this.type}: message N {BitConverter.ToInt32(result.Buffer, 0)}({this.messages.GetCount()}).");
                        }
                        MessageInfo lastMessage = this.messages.Enqueue(result.Buffer);
                    }
                }
                catch (Exception exception)
                {
                    if (this.consoleLog)
                    {
                        Console.WriteLine($"Exception \"{exception.Message}\".");
                    }
                    logger?.Error($"Exception \"{exception.Message}\".");
                    break;
                }
            }
            this.client.Close();
        }
    }
}
