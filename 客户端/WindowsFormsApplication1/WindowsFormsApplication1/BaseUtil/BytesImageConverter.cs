using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsFormsApplication1
{
    class BytesImageConverter
    {
        internal static byte[] image2Bytes(Image source)
        {
            var ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, source);
            ms.Close();
            return ms.ToArray();
        }
        public static Image bytes2Image(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }
    }
}