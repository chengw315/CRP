using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    abstract class CRPBody
    {
        public abstract byte[] getBytes();
        public abstract void readBytes(byte[] body);
    }
}
