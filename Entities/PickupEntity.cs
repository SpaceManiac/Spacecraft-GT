using System;

namespace SpacecraftGT
{
	public class PickupEntity : Entity
	{
		public InventoryItem Item;
		
		public PickupEntity(int x, int y, int z, InventoryItem item)
		{
			X = x; Y = y; Z = z;
			Item = item;
			base.Update();
		}
		
		~PickupEntity() {
			Spacecraft.Log("Destroying " + this);
		}
		
		override public void Update()
		{
			Pair<int, int> pos = CurrentChunk.GetChunkPos((int)X, (int)Z);
			if (CurrentChunk.GetBlock(pos.First, (int)(Y - 0.5), pos.Second) == Block.Air) {
				Y -= 0.5;
			} else {
				Y += 3;
			}
			base.Update();
		}
		
		override public void Despawn()
		{
			base.Despawn();
		}
		
		override public string ToString()
		{
			return "[Entity.Pickup " + EntityID + ": " + Item.Type + "]";
		}
	}
}
