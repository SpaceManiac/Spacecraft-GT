using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacecraftGT
{
    static class ByteClass
    {
        public static byte[] ToBytes(this short S)
        {
            return BitConverter.GetBytes(S);
        }

        public static byte[] ToBytes(this float F)
        {
            byte[] data = BitConverter.GetBytes(F);
            return data;
        }

        public static byte[] ToBytes(this long L)
        {
            byte[] data = BitConverter.GetBytes(L);
            return data;
        }

        public static byte[] ToBytes(this double L)
        {
            byte[] data = BitConverter.GetBytes(L);
            return data;
        }

        public static byte[] ToBytes(this int I)
        {
            byte[] data = BitConverter.GetBytes(I);
            return data;
        }

        public static byte[] ToBytes(this bool B)
        {
            return new byte[] { (byte)(B ? 1 : 0) };
        }

        public static byte[] ToBytes(this byte[] B)
        {
            return B;
        }

        public static byte[] ToBytes(this byte B)
        {
            return new byte[] { B };
        }


        public static byte[] ToBytes(this string S)
        {
            short len = (short) UTF8Encoding.UTF8.GetByteCount(S);
            var B = new Builder<Byte>();
            B.Append(len.ToBytes());
            B.Append(UTF8Encoding.UTF8.GetBytes(S));
            return B.ToArray();
        }
    }
}
