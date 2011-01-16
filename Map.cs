using System;
using System.Collections.Generic;
using NBT;
using System.IO;
using System.IO.Compression;

namespace SpacecraftGT
{
	public class Map
	{
		public string WorldName;
		
		private int _DespawnTimer;
		private Dictionary<long, Chunk> _Chunks;
		private BinaryTag _Structure;
		
		public Map(string Name)
		{
			WorldName = Name;
			_DespawnTimer = 0;
			_Chunks = new Dictionary<long, Chunk>();
			
			StreamReader rawReader = new StreamReader(Name + "/level.dat");
			GZipStream reader = new GZipStream(rawReader.BaseStream, CompressionMode.Decompress);
			_Structure = NbtParser.ParseTagStream(reader);
			reader.Close();
			rawReader.Close();
		}
		
		#region Properties
		
		public int SpawnX { get { return (int)(_Structure["Data"]["SpawnX"].Payload); }
							set { _Structure["Data"]["SpawnX"].Payload = value; } }
		public int SpawnZ { get { return (int)(_Structure["Data"]["SpawnZ"].Payload); }
							set { _Structure["Data"]["SpawnZ"].Payload = value; } }
		public int SpawnY { get { return (int)(_Structure["Data"]["SpawnY"].Payload); }
							set { _Structure["Data"]["SpawnY"].Payload = value; } }
		public long Time  { get { return (long)(_Structure["Data"]["Time"].Payload); }
							set { _Structure["Data"]["Time"].Payload = value; } }
		
		#endregion
		
		public void Generate()
		{
			// ...
		}
		
		public void ForceSave()
		{
			foreach (KeyValuePair<long, Chunk> kvp in _Chunks) {
				kvp.Value.Save();
			}
		}
		
		public void Update()
		{
			List<Entity> entities = new List<Entity>();
			foreach (KeyValuePair<long, Chunk> kvp in new Dictionary<long, Chunk>(_Chunks)) {
				foreach (Entity e in kvp.Value.Entities) {
					entities.Add(e);
				}
			}
			
			// Update entities as needed
			foreach (Entity e in entities) {
				// Spacecraft.Log("Updating " + e);
				e.Update();
			}
			
			// Remove from memory chunks that nobody can see
			_DespawnTimer = (_DespawnTimer + 1) % 20;
			if (_DespawnTimer == 0) {
				List<Chunk> chunksVisible = new List<Chunk>();
				foreach (Entity e in entities) {
					if (e is Player) {
						foreach (Chunk c in GetChunksInRange(e.CurrentChunk)) {
							chunksVisible.Add(c);
						}
					}
				}
				foreach (Chunk c in GetChunksInRange(GetChunkAt(SpawnX, SpawnZ))) {
					chunksVisible.Add(c);
				}
				int i = 0;
				foreach (KeyValuePair<long, Chunk> kvp in new Dictionary<long, Chunk>(_Chunks)) {
					if (!chunksVisible.Contains(kvp.Value)) {
						_Chunks.Remove(kvp.Key);
						++i;
					}
				}
				// Spacecraft.Log("Unloaded " + i + " chunks");
			}
		}
		
		public Chunk GetChunk(int chunkX, int chunkZ)
		{			
			Builder<byte> b = new Builder<byte>();
			b.Append(BitConverter.GetBytes(chunkX));
			b.Append(BitConverter.GetBytes(chunkZ));
			long index = BitConverter.ToInt64(b.ToArray(), 0);
			if (_Chunks.ContainsKey(index)) {
				return _Chunks[index];
			} else {
				return _Chunks[index] = new Chunk(chunkX, chunkZ, this);
			}
		}
		
		public Chunk GetChunkAt(int blockX, int blockZ)
		{
			int xNeg = (blockX < 0 ? 1 : 0), zNeg = (blockZ < 0 ? 1 : 0);
			return GetChunk((int)((blockX + xNeg) / 16) - xNeg, (int)((blockZ + zNeg) / 16) - zNeg);
		}
		
		public List<Chunk> GetChunksInRange(Chunk c)
		{
			List<Chunk> r = new List<Chunk>();
			for (int x = c.ChunkX - 5; x <= c.ChunkX + 5; ++x) {
				for (int z = c.ChunkZ - 5; z <= c.ChunkZ + 5; ++z) {
					if (Math.Abs(c.ChunkX - x) + Math.Abs(c.ChunkZ - z) < 8) {
						r.Add(GetChunk(x, z));
					}
				}
			}
			return r;
		}
		
		public List<Entity> EntitiesIn(Chunk c)
		{
			return c.Entities;
		}
	}
}
