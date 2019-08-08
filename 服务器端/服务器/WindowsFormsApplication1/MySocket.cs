using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class MySocket
    {
        private string name;
        private EndPoint udpAdress;
        private IPAddress myIP;
        private Socket socket;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public EndPoint UdpAdress
        {
            get
            {
                return udpAdress;
            }

            set
            {
                udpAdress = value;
            }
        }

        public MySocket(Socket socket)
        {
            this.socket = socket;
        }

        public void Connect(IPEndPoint end) {
            socket.Connect(end);
        }

        public void startListening(dataHandler handler,System.Action exceptionHandler)
        {
            while (true)
            {
                byte[] data = new byte[1500];
                try
                {
                    socket.Receive(data);
                }
                catch (Exception e) {
                    exceptionHandler.Invoke();
                    break;
                }
                handler.Invoke(this,data);
            }
        }

        public void send(byte[] message)
        {
            this.socket.Send(message);
        }
        public delegate void dataHandler(MySocket socket,byte[] data);

        internal void disConnect()
        {
            if(socket != null)
                this.socket = null;
        }

        public IPAddress getSocketIP() {
            if(myIP == null)
                myIP = ((IPEndPoint)(socket.RemoteEndPoint)).Address;
            return myIP;
        }
        public int getUDPPort() {
            return ((IPEndPoint)UdpAdress).Port;
        }
        public int getTCPPort() {
            return ((IPEndPoint)(socket.RemoteEndPoint)).Port;
        }
    }
}
