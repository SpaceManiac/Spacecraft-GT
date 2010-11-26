using System;
using System.Text;
using NBT;
using System.IO;
using System.IO.Compression;

namespace SpacecraftGT
{
	public class Chunk
	{
		public int ChunkX;
		public int ChunkZ;
		private BinaryTag _Structure;
		private Map _World;
		
		public Chunk(int chunkX, int chunkZ, Map world)
		{
			ChunkX = chunkX;
			ChunkZ = chunkZ;
			_World = world;
			Load();
		}
		
		public void Save()
		{
			StreamWriter RawWriter = new StreamWriter(CalculateFilename());
			GZipStream Writer = new GZipStream(RawWriter.BaseStream, CompressionMode.Compress);
			NbtWriter.WriteTagStream(_Structure, Writer);
			Writer.Close();
		}
		
		public void Load()
		{
			StreamReader RawReader = new StreamReader(CalculateFilename());
			GZipStream Reader = new GZipStream(RawReader.BaseStream, CompressionMode.Decompress);
			_Structure = NbtParser.ParseTagStream(Reader);
			Reader.Close();
			RawReader.Close();
		}
		
		public byte[] GetBytes()
		{
			Builder<Byte> builder = new Builder<Byte>();
			builder.Append((byte[]) _Structure["Level"]["Blocks"].Payload);
			builder.Append((byte[]) _Structure["Level"]["Data"].Payload);
			builder.Append((byte[]) _Structure["Level"]["BlockLight"].Payload);
			builder.Append((byte[]) _Structure["Level"]["SkyLight"].Payload);
			return builder.ToArray();
		}
		
		// ====================
		// Tile gets/sets
		
		public Block GetBlock(int x, int y, int z)
		{
			return (Block) ((byte[])(_Structure["Level"]["Blocks"].Payload))[BlockIndex(x, y, z)];
		}
		
		public void SetBlock(int x, int y, int z, Block block)
		{
			((byte[])(_Structure["Level"]["Blocks"].Payload))[BlockIndex(x, y, z)] = (byte)block;
		}
		
		public byte GetData(int x, int y, int z)
		{
			return 0;
		}
		
		public void SetData(int x, int y, int z, byte data)
		{
			// TODO
		}
		
		public byte GetLight(int x, int y, int z)
		{
			return 0;
		}
		
		public void SetLight(int x, int y, int z, byte data)
		{
			// TODO
		}
		
		public byte GetSkyLight(int x, int y, int z)
		{
			return 0;
		}
		
		public void SetSkyLight(int x, int y, int z, byte data)
		{
			// TODO
		}
		
		// ====================
		// Helper functions
		
		private int BlockIndex(int x, int y, int z)
		{
			return y + (z * 128 + (x * 128 * 16));
		}
		
		private string CalculateFilename()
		{
			int modX = (ChunkX >= 0 ? ChunkX % 64 : 64 - Math.Abs(ChunkX) % 64);
			int modZ = (ChunkZ >= 0 ? ChunkZ % 64 : 64 - Math.Abs(ChunkZ) % 64);
			StringBuilder sb = new StringBuilder();
			return (sb.Append(_World.WorldName).Append("/")
					  .Append(Spacecraft.Base36Encode(modX)).Append("/")
					  .Append(Spacecraft.Base36Encode(modZ)).Append("/")
					  .Append("c.").Append(Spacecraft.Base36Encode(ChunkX))
					  .Append(".").Append(Spacecraft.Base36Encode(ChunkZ))
					  .Append(".dat").ToString());
		}
	}
}
