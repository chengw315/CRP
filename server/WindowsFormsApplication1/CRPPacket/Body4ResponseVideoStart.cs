using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.CRPPacket
{
    class Body4ResponseVideoStart:CRPBody
    {
        private bool isSuccess;
        private string info;

        public bool IsSuccess
        {
            get
            {
                return isSuccess;
            }

            set
            {
                isSuccess = value;
            }
        }

        public string Info
        {
            get
            {
                return info;
            }

            set
            {
                info = value;
            }
        }

        public static Body4ResponseVideoStart nullBody = new Body4ResponseVideoStart();
        private Body4ResponseVideoStart() { }
        public Body4ResponseVideoStart(bool isSuccess, string info = "")
        {
            IsSuccess = isSuccess;
            Info = info;
        }

        public override byte[] getBytes()
        {
            return BytesUtil.catBytes(BitConverter.GetBytes(isSuccess), BytesUtil.string2bytes(info));
        }

        public override void readBytes(byte[] body)
        {
            IsSuccess = BitConverter.ToBoolean(body, 0);
            if (IsSuccess)
                Info = "";
            else
                Info = BytesUtil.bytes2String(BytesUtil.subBytes(body, 1));
        }
    }
}
