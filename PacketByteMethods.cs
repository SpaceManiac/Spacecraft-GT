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
            Index += sizeof(short);
            return S;
        }

        private static int GetNextInt(byte[] data, ref int Index)
        {
            int I = BitConverter.ToInt32(data, Index);
            Index += sizeof(int);
            return I;
        }

        private static long GetNextLong(byte[] data, ref int Index)
        {
            long value = BitConverter.ToInt64(data, Index);
            Index += sizeof(long);
            return value;
        }

        private static float GetNextFloat(byte[] data, ref int Index)
        {
            float value = BitConverter.ToSingle(data, Index);
            Index += sizeof(float);
            return value;
        }

        private static bool GetNextBool(byte[] data, ref int Index)
        {
            byte b = data[Index++];
            return (b == 1);
        }

        private static double GetNextDouble(byte[] data, ref int Index)
        {
            double value = BitConverter.ToDouble(data, Index);
            Index += sizeof(double);
            return value;
        }

        private static Byte GetNextByte(byte[] data, ref int Index)
        {
            return data[Index++];
        }

        private static Byte[] GetNextByteArray(byte[] data, ref int Index)
        {
            throw new NotImplementedException();
        }

    }
}
