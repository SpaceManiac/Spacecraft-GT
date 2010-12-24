using System;

namespace SpacecraftGT
{
	public abstract class Entity
	{
	
		protected double _LastX;
		protected double _LastY;
		protected double _LastZ;
		protected sbyte _LastYaw;
		protected sbyte _LastPitch;
	
		public Chunk CurrentChunk;
		public double X;
		public double Y;
		public double Z;
		public sbyte Yaw;
		public sbyte Pitch;
		public int EntityID;
		
		public Entity()
		{
			CurrentChunk = null;
			EntityID = Spacecraft.Random.Next();
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
