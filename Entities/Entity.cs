using System;

namespace SpacecraftGT
{
	public abstract class Entity
	{
        static Int32 LastID = 0;
        static Int32 NextID
        {
            get
            {
                return ++LastID;
            }
        }
	
		public Chunk CurrentChunk;
		public double X;
		public double Y;
		public double Z;
		public int EntityID;
		
		public Entity()
		{
            EntityID = Entity.NextID;
			CurrentChunk = null;
		}
		
		virtual public void Update()
		{
			Chunk oldChunk = CurrentChunk;
			Chunk newChunk = Spacecraft.Server.World.GetChunkAt((int) X, (int) Z);
			if (oldChunk != newChunk) {
				if (oldChunk != null) oldChunk.Entities.Remove(this);
				newChunk.Entities.Add(this);
				CurrentChunk = newChunk;
			}
		}
		
		public void Remove()
		{
			if (CurrentChunk != null) CurrentChunk.Entities.Remove(this);
		}
		
		override public string ToString()
		{
			return "[Entity " + EntityID + "]";
		}
		
	}
}
