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
			Type = type; Count = 1; Damage = 0;
		}
		
		override public string ToString() {
			string name = "[InvItem ";
			if (Type == -1) {
				name += "Nil";
			} else if (Type < 256) {
				name += "Block." + ((Block)Type).ToString();
			} else {
				name += "Item." + ((Item)Type).ToString();
			}
			if (Type > 0) {
				if (Count != 1) {
					name += " x" + Count;
				}
				if (Damage != 0) {
					name += " d" + Damage;
				}
			}
			name += "]";
			return name;
		}
	}
}