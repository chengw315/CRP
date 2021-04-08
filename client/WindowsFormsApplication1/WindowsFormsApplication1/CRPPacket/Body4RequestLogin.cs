using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.CRPPacket
{
    class Body4RequestLogin:CRPBody
    {
        ushort port;
        string name;

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

        public ushort Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public static Body4RequestLogin nullBody = new Body4RequestLogin();
        private Body4RequestLogin() { }
        public Body4RequestLogin(ushort port,string name) {
            Port = port;
            Name = name;
        }

        public override byte[] getBytes()
        {
            return BytesUtil.catBytes(BitConverter.GetBytes(Port), BytesUtil.string2bytes(Name));
        }

        public override void readBytes(byte[] body)
        {
            Port = BitConverter.ToUInt16(body, 0);
            Name = BytesUtil.bytes2String(BytesUtil.subBytes(body,2));
        }
    }
}
