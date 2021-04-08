using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading;
using WindowsFormsApplication1.CRPPacket;
using WindowsFormsApplication1.VFPacket;

namespace WindowsFormsApplication1
{
    //聊天室客户端
    class CRPClient
    {
        //连接
        private MyTCPSocket tcpSocket;
        private MyUDPSocket udpListener;
        private MyUDPSocket udpSender;
        private int myUDPPort;
        public void setMyUDPPort(int port) {
            myUDPPort = port;
        }
        private EndPoint serverUDPPort;

        //单例模式
        private CRPClient()
        {
            tcpSocket = new MyTCPSocket();
            udpListener = new MyUDPSocket();
            udpSender = new MyUDPSocket();
        }
        public static CRPClient defaultClient = new CRPClient();

        //从服务器收到数据时调用前台的委托
        public delegate void BoolStringHandler(bool isSuccess,string failInfo);
        public delegate void StringHandler(string str);
        public delegate void BitmapHandler(Bitmap ima);
        private StringHandler messageHandler;
        private BoolStringHandler loginHandler;
        private BoolStringHandler startVideoHandler;
        private BitmapHandler videoFrameHandler;
        public void setLoginHandler(BoolStringHandler loginHandler) {
            this.loginHandler = loginHandler;
        }
        public void setMessageHandler(StringHandler loginHandler)
        {
            this.messageHandler = loginHandler;
        }
        public void setVideoFrameHandler(BitmapHandler videoFrame) {
            this.videoFrameHandler = videoFrame;
        }
        public void setStartVideoHandler(BoolStringHandler startVideo) {
            this.startVideoHandler = startVideo;
        }

        //客户端姓名
        private string name;
        public void setName(string name)
        {
            this.name = name;
        }

        //从服务器收数据
        public void listenTcp()
        {
            tcpSocket.startListening(delegate (byte[] data)
            {
                handleTCPData(data);
            });
        }
        public void listenUdp()
        {
            udpListener.Bind(new IPEndPoint(IPAddress.Any, myUDPPort));
            udpListener.startListening(delegate (byte[] data)
            {
                handleUDPData(data);
            });
        }

        //向服务器发数据
        public void login(string name, string ip, int tcpPort, int udpPort)
        {
            CRPConnect(ip, tcpPort, udpPort);
            setName(name);
            sendLoginInfo();
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
                udpSender.send(v, serverUDPPort);
            }
            frameOrder++;
        }
        public void sendRequestVideoStart()
        {
            sendCRPPacket(CRPHead.REQUEST_VIDEO_START, new Body4RequestVideoStart());
        }
        public void sendRequestVideoStop() {
            sendCRPPacket(CRPHead.REQUEST_VIDEO_STOP, new Body4RequestVideoStop());
        }
        public void sendMessage(string message) {
            sendCRPPacket(CRPHead.SEND_MESSAGE, new Body4SendMessage(name+":"+message));
        }

        private void CRPConnect(string ip, int tcpPort, int udpPort)
        {
            serverUDPPort = new IPEndPoint(IPAddress.Parse(ip), udpPort);
            tcpSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), tcpPort));
            new Thread(listenTcp).Start();
            new Thread(listenUdp).Start();      
        }

        private FramePart[] buffer;
        private bool[] isfull;
        private int curFrameOrder = -1;
        private void handleUDPData(byte[] data)
        {
            VFHead head = VFHead.readBytes(BytesUtil.subBytes(data, 0, VFHead.LENGTH));
            if (head.FrameOrder < curFrameOrder) return;
            if (head.FrameOrder > curFrameOrder)
            {
                curFrameOrder = head.FrameOrder;
                buffer = new FramePart[head.PartsNum];
                isfull = new bool[head.PartsNum];
            }
            if (buffer.Length != head.PartsNum)
                return;
            buffer[head.PartOrder] = new FramePart(BytesUtil.subBytes(data, VFHead.LENGTH));
            isfull[head.PartOrder] = true;
            if (ifFull(isfull))
                videoFrameHandler.Invoke(BytesUtil.bytes2Bitmap(BytesUtil.FrameParts2Bytes(buffer)));
        }
        private bool ifFull(bool[] isfull)
        {
            foreach (bool b in isfull) {
                if (!b)
                    return false;
            }
            return true;
        }

        private void handleTCPData(byte[] data)
        {
            byte[] CRPData = BytesUtil.subBytes(data, CRPHead.LENGTH);
            byte[] head = BytesUtil.subBytes(data, 0, CRPHead.LENGTH);
            switch (checkDataType(head))
            {
                case CRPHead.RESPONSE_LOGIN:
                    handleLogin(CRPData);
                    break;
                case CRPHead.FORWARD_MESSAGE:
                    handleMessage(CRPData);
                    break;
                case CRPHead.FORWARD_PICTURE:
                    handlePicture(CRPData);
                    break;
                case CRPHead.RESPONSE_VIDEO_START:
                    handleVideoStart(CRPData);
                    break;
            }
        }

        private void handleVideoStart(byte[] cRPData)
        {
            Body4ResponseVideoStart body = Body4ResponseVideoStart.nullBody;
            body.readBytes(cRPData);
            startVideoHandler.Invoke(body.IsSuccess,body.Info);
        }
        private void handlePicture(byte[] cRPData)
        {
            throw new NotImplementedException();
        }
        private void handleMessage(byte[] data)
        {
            messageHandler.Invoke(BytesUtil.bytes2String(data));
        }
        private void handleLogin(byte[] data)
        {
            Body4ResponseLogin body = Body4ResponseLogin.nullBody;
            body.readBytes(data);
            if (!body.IsSuccess)
            {
                tcpSocket.stopListening();
                udpListener.stopListening();
            }
            loginHandler.Invoke(body.IsSuccess,body.Info);
        }

        private int checkDataType(byte[] head)
        {
            return CRPHead.getType(head);
        }
        private void sendLoginInfo()
        {
            sendCRPPacket(CRPHead.REQUEST_LOGIN, new Body4RequestLogin((ushort)myUDPPort,name));
        }
        private void sendCRPPacket(int headType, CRPBody body) {
            byte[] bodyBytes = body.getBytes();
            byte[] packet = new byte[CRPHead.LENGTH + bodyBytes.Length];
            BytesUtil.Append(packet, CRPHead.getHeadBytes(headType)); 
            BytesUtil.Append(packet, bodyBytes, CRPHead.LENGTH);
            tcpSocket.send(packet);
        }    
    }
}
