using System;

namespace SpacecraftGT
{
	public abstract class Entity
<<<<<<< HEAD
	{
        static Int32 LastID = 0;
        static Int32 NextID
        {
            get
            {
                return ++LastID;
            }
        }


=======
	{
	
		protected double _LastX;
		protected double _LastY;
		protected double _LastZ;
		protected sbyte _LastYaw;
		protected sbyte _LastPitch;
	
		public Chunk CurrentChunk;
>>>>>>> adf2f1b29257cfb4a4970f199a627eb43b076461
		public double X;
		public double Y;
		public double Z;
		public sbyte Yaw;
		public sbyte Pitch;
		public int EntityID;
		
		public Entity()
<<<<<<< HEAD
		{
            EntityID = Entity.NextID;
=======
		{
			CurrentChunk = null;
			EntityID = Spacecraft.Random.Next();
>>>>>>> adf2f1b29257cfb4a4970f199a627eb43b076461
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
			
			double dx = X - _LastX, dy = Y - _LastY, dz = Z - _LastZ;
			bool rotchanged = (Yaw != _LastYaw || Pitch != _LastPitch);
			if (dx != 0 || dy != 0 || dz != 0 || rotchanged) {
				foreach (Player p in Spacecraft.Server.PlayerList) {
					if (p != this && p.VisibleEntities.Contains(this)) p.UpdateEntity(this, dx, dy, dz, rotchanged, false);
				}
			}
			_LastX = X; _LastY = Y; _LastZ = Z;
			_LastYaw = Yaw; _LastPitch = Pitch;
		}
		
		virtual public void Despawn()
		{
			if (CurrentChunk != null) CurrentChunk.Entities.Remove(this);
		}
		
		override public string ToString()
		{
			return "[Entity " + EntityID + "]";
		}
		
	}
}
