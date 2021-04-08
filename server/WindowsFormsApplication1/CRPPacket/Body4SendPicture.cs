using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.CRPPacket
{
    class Body4SendPicture:CRPBody
    {
        private byte offset;
        private string name;
        private Image image;

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
