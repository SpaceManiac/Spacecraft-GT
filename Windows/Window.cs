using System;
using System.Collections.Generic;

namespace SpacecraftGT
{
	public abstract class Window
	{
		private static byte _NextID = 2;
		protected List<Player> _Viewers;
		
		public byte WindowID;
		public byte WindowType;
		public string WindowTitle;
		
		public Window(byte type, string title) {
			_Viewers = new List<Player>();
			WindowID = _NextID;
			if (++_NextID > 125) _NextID = 2;
			
			WindowType = type;
			WindowTitle = title;
		}
		
		virtual public void Open(Player player) {
			_Viewers.Add(player);
			player.OpenWindow(this);
		}
		
		virtual public void Close(Player player) {
			_Viewers.Remove(player);
		}
		
		public abstract bool Click(short slot, byte type, short actionID, InventoryItem item);
		public abstract short Slots();
		
		protected void _SetSlot(short slot, InventoryItem item) {
			foreach (Player player in _Viewers) {
				player.WindowSetSlot(this, slot, item);
			}
		}
	}
}
