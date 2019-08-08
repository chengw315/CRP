using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class MyUDPSocket
    {
        private Socket socket;
        private volatile bool listening;
        private EndPoint myPort;

        public EndPoint MyPort
        {
            get
            {
                return myPort;
            }

            set
            {
                myPort = value;
            }
        }

        public MyUDPSocket()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void startListening(dataHandler handler)
        {
            listening = true;
            while (listening)
            {
                byte[] data = new byte[1456];
                socket.ReceiveFrom(data,ref myPort);
                handler.Invoke(data);
            }
        }

        public void send(byte[] message, EndPoint end)
        {
            socket.SendTo(message,end);
        }
        public delegate void dataHandler(byte[] data);

        internal void close()
        {
            socket.Close();
            listening = false;
        }

        internal void Bind(IPEndPoint iPEndPoint)
        {
            myPort = iPEndPoint;
            socket.Bind(myPort);
        }
    }
}
