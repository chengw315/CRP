using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.BaseUtil
{
    class BytesStringConverter
    {
        public static string bytes2String(byte[] source)
        {
            return System.Text.Encoding.Default.GetString(source);
        }
        public static byte[] string2bytes(string source)
        {
            return System.Text.Encoding.Default.GetBytes(source);
        }
    }
}
