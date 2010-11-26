using System;

namespace SpacecraftGT
{
	public abstract class Entity
	{
		public double X;
		public double Y;
		public double Z;
		public int EntityID;
		
		public Entity()
		{
			EntityID = Spacecraft.Random.Next();
		}
		
	}
}
