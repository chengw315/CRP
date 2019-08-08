using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.CRPPacket
{
    class Body4RequestVideoStart : CRPBody
    {
        public Body4RequestVideoStart() { }

        public override byte[] getBytes()
        {
            return new byte[1];
        }

        public override void readBytes(byte[] body)
        {
            return;
        }
    }
}
