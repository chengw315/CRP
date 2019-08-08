using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.CRPPacket
{
    class Body4ForwardMessage:CRPBody
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

        public static Body4ForwardMessage nullBody = new Body4ForwardMessage();
        private Body4ForwardMessage() { }
        public Body4ForwardMessage(string message)
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
