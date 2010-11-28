using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacecraftGT
{
    abstract partial class Packet
    {
        private static string GetNextString(byte[] data, ref int Index)
        {
            string value = ASCIIEncoding.UTF8.GetString(data, Index + 1, data[Index]);
            Index += value.Length;
            return value;
        }

        private static short GetNextShort(byte[] data, ref int Index)
        {
            short S = BitConverter.ToInt16(data, Index);
            Index += 2;
            return S;
        }

        private static int GetNextInt(byte[] data, ref int Index)
        {
            int I = BitConverter.ToInt32(data, Index);
            Index += 4;
            return I;
        }

        private static long GetNextLong(byte[] data, ref int Index)
        {
            return 0;
        }

        private static float GetNextFloat(byte[] data, ref int Index)
        {
            return 0;
        }

        private static bool GetNextBool(byte[] data, ref int Index)
        {
            throw new NotImplementedException();
        }

        private static double GetNextDouble(byte[] data, ref int Index)
        {
            throw new NotImplementedException();
        }

        private static Byte GetNextByte(byte[] data, ref int Index)
        {
            throw new NotImplementedException();
        }

        private static Byte[] GetNextByteArray(byte[] data, ref int Index)
        {
            throw new NotImplementedException();
        }

    }
}
