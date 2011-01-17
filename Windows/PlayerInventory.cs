using System;

namespace SpacecraftGT
{
	public class PlayerInventory : Window
	{
		private Player _Player;
		
		public PlayerInventory(Player player) : base(0, "Inventory") {
			_Player = player;
			ID = 0;
			slots = new InventoryItem[45];
			for (int i = 0; i < 45; ++i) {
				slots[i].Type = -1;
			}
			_Viewers.Add(_Player);
		}
		
		override public void Open(Player player) { /* nil */ }
		
		override public void Close(Player player) {
			// TODO: Jettison items in crafting slots.
		}
		
		override public bool Click(Player p, short slot, byte type, InventoryItem item) {
			InventoryItem holding = p.WindowHolding;
			if (slot < 0) {
				// outside window
				return false;
			} else if (slot == 0) {
				// crafting slot
				return false;
			} else {
				if (!item.Equals(slots[slot])) return false;
				p.SetHolding(slots[slot]);
				SetSlot(slot, holding);
				return true;
			}
		}
		
		private bool TryAddItem(short i, InventoryItem item) {
			if (slots[i].Type == -1) {
				SetSlot(i, item);
				return true;
			} else if (slots[i].Type == item.Type) {
				if (slots[i].Count + item.Count <= 64) {
					slots[i].Count += item.Count;
					SetSlot(i, slots[i]);
					return true;
				}
			}
			return false;
		}
		
		public bool AddItem(InventoryItem item) {
			// TODO: Better logic.
			for (short i = 36; i < 45; ++i) {
				if (TryAddItem(i, item)) return true;
			}
			for (short i = 9; i < 36; ++i) {
				if (TryAddItem(i, item)) return true;
			}
			return false;
		}
	}
}
