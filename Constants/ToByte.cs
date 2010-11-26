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
            byte[] ret = new byte[S.Length + 1];
            ret[0] = (byte) S.Length;
            for (int i = 0; i < S.Length; i++)
            {
                ret[i + 1] = (byte)S[i];
            }
            return ret;
        }

    }
}
