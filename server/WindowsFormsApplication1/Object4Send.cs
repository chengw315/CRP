using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class Object4Send
    {
        public Object4Send(byte[] b, MySocket m) {
            bytes = b;
            socket = m;
        }

        public byte[] bytes;
        public MySocket socket;
    }
}
