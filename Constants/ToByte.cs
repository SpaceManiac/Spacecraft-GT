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
            B.Append((byte) (S.Length / 255));
            B.Append((byte) (S.Length % 256));
            B.Append(UTF8Encoding.UTF8.GetBytes(S));
            return B.ToArray();
        }

    }
}
