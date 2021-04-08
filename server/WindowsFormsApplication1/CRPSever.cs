using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApplication1.CRPPacket;
using WindowsFormsApplication1.VFPacket;

namespace WindowsFormsApplication1
{
    //聊天室服务器端
    class CRPSever
    {
        public static CRPSever defaultSever = new CRPSever();
        private CRPSever()
        {
            tcpSocketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            udpListener = new MyUDPSocket();
            udpSender = new MyUDPSocket();
        }
        private Socket tcpSocketListener;
        private List<MySocket> tcpSockets = new List<MySocket>();
        private MyUDPSocket udpListener;
        private MyUDPSocket udpSender;
        public void startListening(int TCPPort)
        {
            tcpSocketListener.Bind(new IPEndPoint(IPAddress.Any, TCPPort));
            tcpSocketListener.Listen(10);
            while (true)
            {
                Socket socket = tcpSocketListener.Accept();
                MySocket mySocket = new MySocket(socket);
                new Thread(new ParameterizedThreadStart(startSocketListening)).Start(mySocket);
            }
        }
        private void startSocketListening(object mySocket)
        {         
            ((MySocket)mySocket).startListening(delegate (MySocket mysocket, byte[] data)
            {
                handleTCPData(mysocket, data);
            },delegate() { removeUser((MySocket)mySocket); });
        }
        public void startUDPListening(int udpPort) {
            udpListener.Bind(new IPEndPoint(IPAddress.Any, udpPort));
            udpListener.startListening(delegate (byte[] data) {
                    handleUDPData(data);
            });  
        }
     
        public delegate void mySocketHandler(MySocket socket);
        public delegate void stringHandler(string str);
        public delegate void BitmapHandler(Bitmap ima);
        private stringHandler messageHandler;
        private mySocketHandler addUserHandler;
        private mySocketHandler removeUserHandler;
        private mySocketHandler videoStartHandler;
        private BitmapHandler videoFrameHandler;
        private Action videoStopHandler;

        public void setRemoveUserHandler(mySocketHandler removeUser) {
            this.removeUserHandler = removeUser;
        }
        public void setAddUserHandler(mySocketHandler addUser)
        {
            this.addUserHandler = addUser;
        }
        public void setVideoStopHandler(Action videoStop) {
            this.videoStopHandler = videoStop;
        }
        public void setVideoStartHandler(mySocketHandler videoStart) {
            this.videoStartHandler = videoStart;
        }
        public void setMessageHandler(stringHandler message) {
            this.messageHandler = message;
        }
        public void setVideoFrameHandler(BitmapHandler videoFrame)
        {
            videoFrameHandler = videoFrame;
        }

        public void forwardMessage(string message)
        {
            foreach (MySocket sock1 in tcpSockets)
                sendCRPPacket(sock1, CRPHead.FORWARD_MESSAGE, new Body4ForwardMessage(message));
        }
        private void responseLogin(MySocket socket, bool isSuccess, string failInfo = "")
        {
            sendCRPPacket(socket, CRPHead.RESPONSE_LOGIN, new Body4ResponseLogin(isSuccess, failInfo));
        }

        private void handleTCPData(MySocket mySocket,byte[] data)
        {
            byte[] CRPData = BytesUtil.subBytes(data, CRPHead.length);
            byte[] head = BytesUtil.subBytes(data, 0, CRPHead.length);
            switch (checkDataType(head)) {
                case CRPHead.REQUEST_LOGIN:
                    handleLogin(mySocket,CRPData);
                    break;
                case CRPHead.REQUEST_LOGOUT:
                    handleLogout(mySocket,CRPData);
                    break;
                case CRPHead.SEND_MESSAGE:
                    handleMessage(mySocket,CRPData);
                    break;
                case CRPHead.SEND_PICTURE:
                    handlePicture(mySocket,CRPData);
                    break;
                case CRPHead.REQUEST_VIDEO_START:
                    handleVideoStart(mySocket,CRPData);
                    break;
                case CRPHead.REQUEST_VIDEO_STOP:
                    handleVideoStop(mySocket,CRPData);
                    break;
            }
        }

        private const ushort UDPDATALENGTH = 1450;
        private ushort frameOrder = 0;
        public void sendVideoFrame(Bitmap frame)
        {
            ushort partOrder = 0;
            byte[] frameBytes = BytesUtil.bitmap2Bytes(frame);
            ushort partsNum = (ushort)(frameBytes.Length / UDPDATALENGTH + ((frameBytes.Length % UDPDATALENGTH == 0) ? 0 : 1));
            List<byte[]> frameList = BytesUtil.divideBytes(frameBytes, UDPDATALENGTH);
            foreach (byte[] framePart in frameList)
            {
                var v = BytesUtil.catBytes(new VFHead(frameOrder, partsNum, partOrder++).getBytes(), framePart);
                foreach(MySocket socket in tcpSockets)
                    udpSender.send(v, socket.UdpAdress);
            }
            frameOrder++;
        }

        private FramePart[] buffer;
        private bool[] isfull;
        private int curFrameOrder;
        private void initVideo() {
            curFrameOrder = -1;
        }
        private void handleUDPData(byte[] data)
        {
            foreach (MySocket socket in tcpSockets)
                ThreadPool.QueueUserWorkItem(new WaitCallback(send4ThreadPool), new Object4Send(data, socket));
            VFHead head = VFHead.readBytes(BytesUtil.subBytes(data, 0, VFHead.LENGTH));
            if (head.FrameOrder < curFrameOrder) return;
            if (head.FrameOrder > curFrameOrder)
            {
                curFrameOrder = head.FrameOrder;
                buffer = new FramePart[head.PartsNum];
                isfull = new bool[head.PartsNum];
            }
            if(buffer.Length != head.PartsNum)
                return;
            buffer[head.PartOrder] = new FramePart(BytesUtil.subBytes(data, VFHead.LENGTH));
            isfull[head.PartOrder] = true;
            if (ifFull(isfull))
                videoFrameHandler.Invoke(BytesUtil.bytes2Bitmap(BytesUtil.FrameParts2Bytes(buffer)));
        }
        private void send4ThreadPool(object obje) {
            Object4Send obj = (Object4Send)obje;
            udpSender.send(obj.bytes, obj.socket.UdpAdress);
        }

        private bool ifFull(bool[] isfull)
        {
            foreach (bool b in isfull)
            {
                if (!b)
                    return false;
            }
            return true;
        }

        private int checkDataType(byte[] head)
        {
            return CRPHead.getType(head);
        }
        private void handleVideoStop(MySocket mySocket, byte[] cRPData)
        {
            livingName = "";
            videoStopHandler.Invoke();
        }
        private string livingName = "";
        private void handleVideoStart(MySocket mySocket, byte[] cRPData)
        {
            if(livingName!="") {
                sendCRPPacket(mySocket, CRPHead.RESPONSE_VIDEO_START, new Body4ResponseVideoStart(false, "直播功能被" + livingName + "占用"));
                return;
            }
            initVideo();
            livingName = mySocket.Name;
            videoStartHandler.Invoke(mySocket);
            sendCRPPacket(mySocket,CRPHead.RESPONSE_VIDEO_START,new Body4ResponseVideoStart(true));
        }
        private void handlePicture(MySocket mySocket, byte[] cRPData)
        {
            throw new NotImplementedException();
        }
        private void handleMessage(MySocket mySocket, byte[] cRPData)
        {
            Body4SendMessage messageBody = Body4SendMessage.nullBody;
            messageBody.readBytes(cRPData);
            messageHandler.Invoke(messageBody.Message);
            forwardMessage(messageBody.Message);
        }
        private void handleLogout(MySocket mySocket, byte[] cRPData)
        {
            removeUser(mySocket);
        }

        private void removeUser(MySocket mySocket)
        {
            tcpSockets.Remove(mySocket);
            mySocket.disConnect();
            removeUserHandler.Invoke(mySocket);
            mySocket = null;
        }

        private void handleLogin(MySocket mySocket, byte[] cRPData)
        {
            Body4RequestLogin body = Body4RequestLogin.nullBody;
            body.readBytes(cRPData);
            string name = body.Name;
            foreach (MySocket sock1 in tcpSockets)
            {
                if (sock1.Name == name)
                {
                    responseLogin(mySocket, false, "名字已被使用");
                    return;
                }
            }
            mySocket.Name = name;
            mySocket.UdpAdress = new IPEndPoint(mySocket.getSocketIP(),body.Port);
            tcpSockets.Add(mySocket);
            addUserHandler.Invoke(mySocket);
            responseLogin(mySocket, true);
            string message = name + "进入聊天室";
            forwardMessage(message);
        }

        private void sendCRPPacket(MySocket tcpSocket,int headType, CRPBody body)
        {
            byte[] bodyBytes = body.getBytes();
            byte[] packet = new byte[CRPHead.length + bodyBytes.Length];
            BytesUtil.Append(packet, CRPHead.getHeadBytes(headType));
            BytesUtil.Append(packet, bodyBytes, CRPHead.length);
            tcpSocket.send(packet);
        }
    }
}
