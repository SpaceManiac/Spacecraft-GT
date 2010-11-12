using System;
using System.Collections.Generic;

namespace SpacecraftGT
{
	public class Map
	{
		public string WorldName;
		
		private Dictionary<Pair<int, int>, Chunk> _Chunks;
		
		public Map(string Name)
		{
			WorldName = Name;
			_Chunks = new Dictionary<Pair<int, int>, Chunk>();
		}
		
		public void Generate()
		{
			// ...
		}
		
		public void ForceSave()
		{
			foreach(KeyValuePair<Pair<int, int>, Chunk> kvp in _Chunks) {
				kvp.Value.Save();
			}
		}
		
		public Chunk GetChunk(int chunkX, int chunkZ)
		{
			Pair<int, int> pair = new Pair<int, int>(chunkX, chunkZ);
			if (!_Chunks.ContainsKey(pair)) {
				_Chunks[pair] = new Chunk(chunkX, chunkZ, this);
			}
			return _Chunks[pair];
		}
		
		public Chunk GetChunkAt(int blockX, int blockZ)
		{
			return GetChunk((int)(blockX / 16) - (blockX < 0 ? 1 : 0), (int)(blockZ / 16) - (blockZ < 0 ? 1 : 0));
		}
	}
}
