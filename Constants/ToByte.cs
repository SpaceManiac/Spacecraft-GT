using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacecraftGT.Constants
{
    static class ByteClass
    {

        static byte[] ToBytes(this short S)
        {
            // Big-Endian because that's what Minecraft uses. 
            return new byte[] { 
                (byte)(S / 256), 
                (byte)(S % 256) 
            };
        }

        static byte[] ToBytes(this string S)
        {
            short len = (short) UTF8Encoding.UTF8.GetByteCount(S);
            var B = new Builder<Byte>();
            B.Append(len.ToBytes());
            B.Append(UTF8Encoding.UTF8.GetBytes(S));
            return B.ToArray();
        }

    }
}
