using System;
using System.Diagnostics;
using System.Security.Authentication;
using WebSocket4Net;

namespace ProviderContract.PropSocket
{
    public delegate void PropSocketEventHandler(PropSocket socket, PropSocketEventArgs e);

    public class PropSocket : IDisposable
    {
        private WebSocket socket;
        public int Id;

        public PropSocket(string wss, SslProtocols protocols, int id)
        {
            socket = new WebSocket(wss, sslProtocols: protocols);
            Id = id;

            socket.Closed += SocketClosed;
            socket.DataReceived += SocketDataReceived;
            socket.Error += SocketErrorMessage;
            socket.MessageReceived += SocketMessageReceived;
            socket.Opened += SocketOpened;
        }

        public bool IsConnected => socket.State == WebSocketState.Open;

        public int AutoSendPingIntervalSeconds
        {
            get => socket.AutoSendPingInterval;
            set => socket.AutoSendPingInterval = value;
        }

        public bool EnableAutoSendPing
        {
            get => socket.EnableAutoSendPing;
            set => socket.EnableAutoSendPing = value;
        }

        public event PropSocketEventHandler Closed;
        public event PropSocketEventHandler CompressedDataReceived;
        public event PropSocketEventHandler Error;
        public event PropSocketEventHandler MessageReceived;
        public event PropSocketEventHandler Opened;

        private void SocketClosed(object sender, EventArgs e)
        {
            PropSocketEventArgs args = new PropSocketEventArgs();
            Closed?.Invoke(this, args);
        }
        private void SocketDataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            PropSocketEventArgs args = new PropSocketEventArgs()
            {
                CompressedData = e.Data
            };
            CompressedDataReceived?.Invoke(this, args);
        }
        private void SocketErrorMessage(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            PropSocketEventArgs args = new PropSocketEventArgs()
            {
                Exception = e.Exception
            };
            Error?.Invoke(this, args);
        }
        private void SocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            PropSocketEventArgs args = new PropSocketEventArgs()
            {
                Message = e.Message
            };
            MessageReceived?.Invoke(this, args);
        }
        private void SocketOpened(object sender, EventArgs e)
        {
            PropSocketEventArgs args = new PropSocketEventArgs();
            Opened?.Invoke(this, args);
        }

        public void Close()
        {
            socket.Close();
        }
        public void Dispose()
        {
            if (socket != null)
            {
                socket.Dispose();
            }
            GC.SuppressFinalize(this);
        }
        public void Open()
        {
            socket.Open();
        }

        public void Send(string message)
        {
            socket.Send(message);
        }

        ~PropSocket()
        {
            Dispose();
        }
    }
}
