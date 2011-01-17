using System;

namespace SpacecraftGT
{
	public class PickupEntity : Entity
	{
		public InventoryItem Item;
		
		public PickupEntity(int x, int y, int z, InventoryItem item)
		{
			X = x + .5; Y = y + .5; Z = z + .5;
			Item = item;
			base.Update();
		}
		
		override public void Update()
		{
			Pair<int, int> pos = CurrentChunk.GetChunkPos(X, Z);
			if (CurrentChunk.GetBlock(pos.First, (int)(Y - 0.25), pos.Second) == Block.Air) {
				Y -= 0.2;
			}
			base.Update();
			foreach (Player p in Spacecraft.Server.PlayerList) {
				if (Math.Abs(p.X - X) < 1 && Math.Abs(p.Z - Z) < 1 && Y <= p.Y + 2 && Y >= p.Y) {
					if (p.Inventory.AddItem(Item)) {
						foreach (Player p2 in Spacecraft.Server.PlayerList) {
							if (p2.VisibleEntities.Contains(this)) p2.PickupCollected(this, p);
						}
						Despawn();
						break;
					}
				}
			}
		}
		
		override public void Despawn()
		{
			base.Despawn();
		}
		
		override public string ToString()
		{
			return "[Entity.Pickup " + EntityID + ": " + Item + "]";
		}
	}
}
