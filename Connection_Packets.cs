using System;
using System.IO;
using zlib;

namespace SpacecraftGT
{
    public partial class Connection
    {

        
        public void SpawnNamedEntity(int EntityID, string PlayerName, int X, int Y, int Z, byte Rotation, byte Pitch, short Item)
        {
            throw new NotImplementedException();
        }

        public void SendChunk(Chunk chunk)
        {
            Transmit(PacketType.PreChunk, chunk.ChunkX, chunk.ChunkZ, (sbyte)1);

            byte[] uncompressed = chunk.GetBytes();
            MemoryStream mem = new MemoryStream();
            ZOutputStream stream = new ZOutputStream(mem, zlibConst.Z_BEST_COMPRESSION);
            stream.Write(uncompressed, 0, uncompressed.Length);
            stream.Close();
            byte[] data = mem.ToArray();

            Transmit(PacketType.MapChunk, 16 * chunk.ChunkX, (short)0, 16 * chunk.ChunkZ,
                (sbyte)15, (sbyte)127, (sbyte)15, data.Length, data);
        }

        public void Disconnect(string message)
        {
            Transmit(PacketType.Disconnect, message);
        }
    }
}