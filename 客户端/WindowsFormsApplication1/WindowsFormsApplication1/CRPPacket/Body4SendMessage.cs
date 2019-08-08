using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.CRPPacket
{
    class Body4SendMessage:CRPBody
    {
        string message;

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }

        public static Body4SendMessage nullBody = new Body4SendMessage();
        private Body4SendMessage() { }
        public Body4SendMessage(string message)
        {
            this.Message = message;
        }

        public override byte[] getBytes()
        {
            return BytesUtil.string2bytes(Message);
        }

        public override void readBytes(byte[] body)
        {
            this.Message = BytesUtil.bytes2String(body);
        }
    }
}
