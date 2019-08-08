using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class MyTCPSocket
    {
        private Socket socket;
        private volatile bool listening;

        public MyTCPSocket()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(IPEndPoint end) {
            if(!socket.Connected)
                socket.Connect(end);
        }

        public void startListening(dataHandler handler)
        {
            listening = true;
            while (listening)
            {
                byte[] data = new byte[1500];
                socket.Receive(data);
                handler.Invoke(data);
            }
        }
        public void stopListening() {
            listening = false;
        }
        public void send(byte[] message)
        {
            this.socket.Send(message);
        }
        public delegate void dataHandler(byte[] data);

        internal void disConnect()
        {
            this.socket.Disconnect(true);
            listening = false;
        }
    }
}
