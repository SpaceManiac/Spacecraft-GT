using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacecraftGT.Constants
{
    static class ByteClass
    {
        static byte[] ToBytes(this string S)
        {
            var B = new Builder<Byte>();
            B.Append((byte)(UTF8Encoding.UTF8.GetByteCount(S) / 255));
            B.Append((byte)(UTF8Encoding.UTF8.GetByteCount(S) % 256));
            B.Append(UTF8Encoding.UTF8.GetBytes(S));
            return B.ToArray();
        }

    }
}
