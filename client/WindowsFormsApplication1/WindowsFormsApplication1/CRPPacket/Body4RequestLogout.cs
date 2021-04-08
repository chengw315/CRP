using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.CRPPacket
{
    class Body4RequestLogout : CRPBody
    {

        public static Body4RequestLogout nullBody = new Body4RequestLogout();
        private Body4RequestLogout() { }
        
        public override byte[] getBytes()
        {
            throw new NotImplementedException();
        }

        public override void readBytes(byte[] body)
        {
            throw new NotImplementedException();
        }
    }
}
