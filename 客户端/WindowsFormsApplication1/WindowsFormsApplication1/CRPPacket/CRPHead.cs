using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class CRPHead
    {
        public const int LENGTH = 2;

        //标识符
        private const byte ID4CRP = 122;

        private const int NOTCRP = -1;
        public const int REQUEST_LOGIN = 0;
        public const int RESPONSE_LOGIN = 128;
        public const int REQUEST_LOGOUT = 4;
        public const int REQUEST_VIDEO_START = 1;
        public const int RESPONSE_VIDEO_START = 129;
        public const int REQUEST_VIDEO_STOP = 5;
        public const int SEND_MESSAGE = 2;
        public const int FORWARD_MESSAGE = 130;
        public const int SEND_PICTURE = 3;
        public const int FORWARD_PICTURE = 131;


        public static byte[] getHeadBytes(int type)
        {
            byte[] result = new byte[LENGTH];
            result[0] = ID4CRP;
            result[1] = (byte)type;
            return result;
        }
        public static int getType(byte[] head)
        {
            return head[0] == ID4CRP ? head[1] : NOTCRP;
        }
    }
}
