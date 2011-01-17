using System;
using System.Collections.Generic;

namespace SpacecraftGT
{
	public abstract class Window
	{
		private static byte _NextID = 2;
		protected List<Player> _Viewers;
		
		public byte ID;
		public byte Type;
		public string Title;
		
		public InventoryItem[] slots { get; protected set; }
		
		protected Window(byte type, string title) {
			_Viewers = new List<Player>();
			ID = _NextID;
			if (++_NextID > 125) _NextID = 2;
			
			Type = type;
			Title = title;
		}
		
		virtual public void Open(Player player) {
			_Viewers.Add(player);
			player.OpenWindow(this);
		}
		
		virtual public void Close(Player player) {
			_Viewers.Remove(player);
			if (_Viewers.Count == 0) {
				Spacecraft.Server.WindowList.Remove(this);
			}
		}
		
		public abstract bool Click(Player p, short slot, byte type, InventoryItem item);
		
		public void SetSlot(short slot, InventoryItem item) {
			slots[slot] = item;
			foreach (Player player in _Viewers) {
				player.WindowSetSlot(this, slot, item);
			}
		}
	}
}
