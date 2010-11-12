using System;
using System.Text;

namespace SpacecraftGT
{
	public class Chunk
	{
		public int ChunkX;
		public int ChunkZ;
		private Map _World;
		
		public Chunk(int chunkX, int chunkZ, Map world)
		{
			ChunkX = chunkX;
			ChunkZ = chunkZ;
			_World = world;
		}
		
		public void Save()
		{
			string filename = CalculateFilename();
		}
		
		public void Load()
		{
			string filename = CalculateFilename();
		}
		
		public string CalculateFilename() {
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
