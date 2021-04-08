using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication1.BaseUtil;

namespace WindowsFormsApplication1
{
    class BytesUtil
    {
        public static void Append(byte[] source, byte[] addtion, int offset = 0)
        {
            int addtionOffset = 0;
            while (addtionOffset < addtion.Length)
                source[offset++] = addtion[addtionOffset++];
        }
        public static byte[] subBytes(byte[] source, int offset, int length = -1)
        {
            if (length == -1)
                length = source.Length - offset;
            byte[] result = new byte[length];
            int resultOffset = 0;
            while (resultOffset < length)
                result[resultOffset++] = source[offset++];
            return result;
        }

        public static byte[] FrameParts2Bytes(FramePart[] frameParts)
        {
            int length = 0;
            foreach (FramePart b in frameParts)
                length += b == null ? FramePart.DEFAULTLENGTH : b.bytes.Length;
            byte[] result = new byte[length];
            int offset = 0;
            foreach (FramePart b in frameParts)
            {
                if (b == null)
                    continue;
                Append(result, b.bytes, offset);
                offset += b.bytes.Length;
            }
            return result;
        }

        public static byte[] catBytes(params byte[][] bytes)
        {
            int length = 0;
            foreach (byte[] b in bytes)
                length += b.Length;
            byte[] result = new byte[length];
            int offset = 0;
            foreach (byte[] b in bytes)
            {
                Append(result, b, offset);
                offset += b.Length;
            }
            return result;
        }
        public static List<byte[]> divideBytes(byte[] v, int partLength)
        {
            List<byte[]> result = new List<byte[]>();
            int offset = 0;
            for (; offset < v.Length - partLength; offset += partLength)
            {
                result.Add(subBytes(v, offset, partLength));
            }
            if (offset < v.Length - 1)
                result.Add(subBytes(v, offset));
            return result;
        }

        public static string bytes2String(byte[] source)
        {
            return BytesStringConverter.bytes2String(source);
        }
        public static byte[] string2bytes(string source)
        {
            if (source == null || source == "")
                return new byte[0];
            return BytesStringConverter.string2bytes(source);
        }

        public static byte[] bitmap2Bytes(Bitmap source)
        {
            return BytesBitmapConverter.bitmap2Bytes(source);
        }
        public static Bitmap bytes2Bitmap(byte[] source)
        {
            return BytesBitmapConverter.bytes2Bitmap(source);
        }

        public static byte[] Image2Bytes(Image source)
        {
            return BytesImageConverter.image2Bytes(source);
        }
        public static Image bytes2Image(byte[] source)
        {
            return BytesImageConverter.bytes2Image(source);
        }
    }
}
