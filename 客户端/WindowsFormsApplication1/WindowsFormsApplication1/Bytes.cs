using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class FramePart
    {
        public byte[] bytes;
        public const int DEFAULTLENGTH = 1450;

        public FramePart(byte[] v)
        {
            this.bytes = v;
        }
    }
}
