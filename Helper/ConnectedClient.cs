using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Baseapp.Helper
{
    public class ConnectedClient
    {
        private Socket client;
        private TcpClient TCPclient;
        private byte[] _buffer = new byte[4096];
        public bool Connected { get; private set; }

        public event EventHandler<string> OnDataReceived;

        public bool Connect(string ip, int port)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(ip, port);
                Connected = true;
                StartReceive();
                return true;
            }
            catch
            {
                Connected = false;
                return false;
            }
        }

        private void StartReceive()
        {
            Task.Run(() =>
            {
                while (Connected)
                {
                    try
                    {
                        int len = client.Receive(_buffer);
                        if (len > 0)
                        {
                            string data = Encoding.ASCII.GetString(_buffer, 0, len);
                            OnDataReceived?.Invoke(this, data);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }


        public string SendAndReceive(string data)
        {
            if (!Connected)
                return "Not Connected";

            Send(data);

            byte[] buffer = new byte[1024];
            int size = client.Receive(buffer);
            return Encoding.ASCII.GetString(buffer, 0, size);
        }

        public void Send(string data)
        {
            if (!Connected)
                return;

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            client.Send(bytes);
        }
    }
}
