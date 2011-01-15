using System;

namespace SpacecraftGT
{
	public class PickupEntity : Entity
	{
		public InventoryItem Item;
		
		public PickupEntity(int x, byte y, int z, InventoryItem item)
		{
			X = x; Y = y; Z = z;
			Item = item;
		}
		
		override public void Update()
		{
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
