using System;

namespace SpacecraftGT
{
	public struct InventoryItem
	{
		public short Type;
		public byte Count;
		public short Damage;
		
		public InventoryItem(short type, byte count, short damage) {
			Type = type; Count = count; Damage = damage;
		}
		public InventoryItem(short type) {
			Type = type; Count = 0; Damage = 0;
		}
	}
}