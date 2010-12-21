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


		public double X;
		public double Y;
		public double Z;
		public int EntityID;
		
		public Entity()
		{
            EntityID = Entity.NextID;
		}
		
	}
}
