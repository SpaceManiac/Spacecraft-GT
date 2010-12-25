using System;
using System.Text;
using NBT;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace SpacecraftGT
{
	public class Chunk
	{
		public int ChunkX;
		public int ChunkZ;
		public List<Entity> Entities;
		
		private BinaryTag _Structure;
		private Map _World;
		
		public Chunk(int chunkX, int chunkZ, Map world)
		{
			ChunkX = chunkX;
			ChunkZ = chunkZ;
			Entities = new List<Entity>();
			_World = world;
			Load();
		}
		
		public void Generate()
		{
			byte[] blocks = new byte[32768], data = new byte[16384];
			byte[] skylight = new byte[16384], light = new byte[16384];
			byte[] height = new byte[256];
			BinaryTag[] entities = new BinaryTag[0], tileEntities = new BinaryTag[0];
			
			for (int i = 0; i < 16348; ++i) {
				blocks[i*2] = (byte) Block.Rock;
				blocks[i*2 + 1] = (byte) Block.Rock;
				skylight[i] = 0xFF;
				light[i] = 0xFF;
			}
			
			BinaryTag[] structure = new BinaryTag[] {
				new BinaryTag(TagType.ByteArray, blocks, "Blocks"),
				new BinaryTag(TagType.ByteArray, data, "Data"),
				new BinaryTag(TagType.ByteArray, skylight, "SkyLight"),
				new BinaryTag(TagType.ByteArray, light, "BlockLight"),
				new BinaryTag(TagType.ByteArray, height, "HeightMap"),
				new BinaryTag(TagType.List, entities, "Entities"),
				new BinaryTag(TagType.List, tileEntities, "TileEntities"),
				new BinaryTag(TagType.Long, (long) 0, "LastUpdate"),
				new BinaryTag(TagType.Int, (int) ChunkX, "xPos"),
				new BinaryTag(TagType.Int, (int) ChunkZ, "zPos"),
				new BinaryTag(TagType.Byte, (byte) 0, "TerrainPopulated")
			};
			
			_Structure = new BinaryTag(TagType.Compound, new BinaryTag[] {
				new BinaryTag(TagType.Compound, structure, "Level")
			});
			Save();
		}
		
		public void Save()
		{
			string filename = CalculateFilename();
			int i = filename.LastIndexOfAny(new char[] { '/', '\\', ':' });
			Directory.CreateDirectory(filename.Substring(0, i));
			
			StreamWriter rawWriter = new StreamWriter(filename);
			GZipStream writer = new GZipStream(rawWriter.BaseStream, CompressionMode.Compress);
			NbtWriter.WriteTagStream(_Structure, writer);
			writer.Close();
		}
		
		public void Load()
		{
			try {
				StreamReader rawReader = new StreamReader(CalculateFilename());
				GZipStream reader = new GZipStream(rawReader.BaseStream, CompressionMode.Decompress);
				_Structure = NbtParser.ParseTagStream(reader);
				reader.Close();
				rawReader.Close();
			}
			catch (FileNotFoundException) {
				Generate();
			}
			catch (DirectoryNotFoundException) {
				Generate();
			}
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
		
		public Pair<int, int> GetChunkPos(int x, int z)
		{
			Spacecraft.Log(this + ".GetChunkPos(" + x + "," + z + "):");
			//int newX = 
			return new Pair<int, int>(0, 0);
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
		
		override public string ToString()
		{
			return "[Chunk at " + ChunkX + ", " + ChunkZ + "]";
		}
		
	}
}
