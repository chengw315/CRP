using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class BytesBitmapConverter
    {
        public static byte[] bitmap2Bytes(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            byte[] result = ms.GetBuffer();
            ms.Close();
            return result;
        }
        public static Bitmap bytes2Bitmap(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            Bitmap result = (Bitmap)Image.FromStream(ms);
            ms.Close();
            return result;
        }
    }
}
