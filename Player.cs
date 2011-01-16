using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace SpacecraftGT
{
	public class Player : Entity
	{
		private Connection _Conn;
		public string Username;
		public bool Spawned;
		
		public List<Chunk> VisibleChunks;
		public List<Entity> VisibleEntities;
		
		public Player(TcpClient client)
		{
			_Conn = new Connection(client, this);
			Username = "";
			Spawned = false;
			CurrentChunk = null;
			VisibleChunks = new List<Chunk>();
			VisibleEntities = new List<Entity>();
		}
		
		#region Entity Overrides
		
		public void Spawn()
		{
			Spacecraft.Server.Spawn(this);
			Spawned = true;
			CurrentChunk = null;
			X = Spacecraft.Server.World.SpawnX + 0.5;
			Y = Spacecraft.Server.World.SpawnY + 3;
			Z = Spacecraft.Server.World.SpawnZ + 0.5;
			Update();
			_Conn.Transmit(PacketType.SpawnPosition, (int)X, (int)Y, (int)Z);
			_Conn.Transmit(PacketType.PlayerPositionLook, X, Y, Y, Z, (float) 0, (float) 0, (sbyte) 1);
		}
		
		override public void Despawn()
		{
			if (!Spawned) return;
			Spawned = false;
			Spacecraft.Server.Despawn(this);
			base.Despawn();
		}
		
		public override void Update()
		{
			if (!Spawned) return;
			Chunk newChunk = Spacecraft.Server.World.GetChunkAt((int)X, (int)Z);
			
			if (newChunk != CurrentChunk) {
				List<Chunk> newVisibleChunks = new List<Chunk>();
				
				foreach (Chunk c in Spacecraft.Server.World.GetChunksInRange(newChunk)) {
					newVisibleChunks.Add(c);
				}
				foreach (Chunk c in VisibleChunks) {
					if (!newVisibleChunks.Contains(c)) {
						_Conn.Transmit(PacketType.PreChunk, c.ChunkX, c.ChunkZ, (sbyte) 0);
					}
				}
				foreach (Chunk c in newVisibleChunks) {
					if (!VisibleChunks.Contains(c)) {
						_Conn.SendChunk(c);
					}
				}
				
				VisibleChunks = newVisibleChunks;
			}
			
			List<Entity> newVisibleEntities = new List<Entity>();
			foreach (Chunk c in VisibleChunks) {
				foreach (Entity e in c.Entities) {
					newVisibleEntities.Add(e);
				}
			}
			foreach (Entity e in VisibleEntities) {
				if (!newVisibleEntities.Contains(e)) {
					DespawnEntity(e);
				}
			}
			foreach (Entity e in newVisibleEntities) {
				if (!VisibleEntities.Contains(e)) {
					SpawnEntity(e);
				}
			}
			VisibleEntities = newVisibleEntities;
			
			_Conn.Transmit(PacketType.TimeUpdate, Spacecraft.Server.World.Time);
			base.Update();
		}
		
		override public string ToString()
		{
			return "[Entity.Player " + EntityID + ": " + Username + "]";
		}
		
		#endregion
		
		#region Connection Interface: Misc
		
		public void SendMessage(string message)
		{
			_Conn.Transmit(PacketType.Message, message);
		}
		
		public void BlockChanged(int x, int y, int z, Block block)
		{
			_Conn.Transmit(PacketType.BlockChange, x, (sbyte) y, z, (sbyte) block, (sbyte) 0);
		}
		
		public void RecvMessage(string message)
		{
			Spacecraft.Log("<" + Username + "> " + message);
			Spacecraft.Server.MessageAll("<" + Username + "> " + message);
		}
		
		public void Disconnect(string message)
		{
			_Conn.Disconnect(message);
		}
		
		#endregion
		
		#region Connection Interface: Entities
		
		public void UpdateEntity(Entity e, double dx, double dy, double dz, bool rotchanged, bool forceabs)
		{
			if (!Spawned) return;
			if (dx == 0 && dy == 0 && dz == 0) {
				if (rotchanged) {
					_Conn.Transmit(PacketType.EntityLook, e.EntityID, (sbyte) e.Yaw, (sbyte) e.Pitch);
				}
			} else if (Math.Abs(dx) < 4 && Math.Abs(dy) < 4 && Math.Abs(dz) < 4 && !forceabs) {
				if (rotchanged) {
					_Conn.Transmit(PacketType.EntityLookAndMove, e.EntityID,
						(sbyte) (dx * 32), (sbyte) (dy * 32), (sbyte) (dz * 32),
						(sbyte) e.Yaw, (sbyte) e.Pitch);
				} else {
					_Conn.Transmit(PacketType.EntityRelativeMove, e.EntityID,
						(sbyte) (dx * 32), (sbyte) (dy * 32), (sbyte) (dz * 32));
				}
			} else {
				_Conn.Transmit(PacketType.EntityTeleport, e.EntityID,
					(int) (e.X * 32), (int) (e.Y * 32), (int) (e.Z * 32),
					(sbyte) e.Yaw, (sbyte) e.Pitch);
			}
		}
		
		#endregion
		
		#region Connection Interface: Windows
		
		public void OpenWindow(Window window) {
			_Conn.Transmit(PacketType.OpenWindow, (sbyte) window.WindowID, (sbyte) window.WindowType,
				window.WindowTitle, (sbyte) window.Slots());
		}
		
		public void WindowSetSlot(Window window, short slot, InventoryItem item) {
			_Conn.Transmit(PacketType.WindowSetSlot, (sbyte) window.WindowID, slot, item);
		}
		
		#endregion
		
		#region Helpers
		
		private void DespawnEntity(Entity e)
		{
			if (!Spawned || e == this) return;
			_Conn.Transmit(PacketType.DestroyEntity, e.EntityID);
		}
		
		private void SpawnEntity(Entity e)
		{
			if (!Spawned || e == this) return;
			
			if (e is Player) {
				Player p = (Player) e;
				_Conn.Transmit(PacketType.NamedEntitySpawn, p.EntityID,
					p.Username, (int)(p.X * 32), (int)(p.Y * 32), (int)(p.Z * 32),
					p.Yaw, p.Pitch, (short) Block.Brick);
			} else if (e is PickupEntity) {
				PickupEntity p = (PickupEntity) e;
				_Conn.Transmit(PacketType.PickupSpawn, p.EntityID,
					p.Item.Type, (sbyte) p.Item.Count, p.Item.Damage,
					(int)(p.X * 32), (int)(p.Y * 32), (int)(p.Z * 32),
					p.Yaw, p.Pitch, (sbyte) 0);
			} else {
				SendMessage(Color.Purple + "Spawning " + e);
				return;
			}
			_Conn.Transmit(PacketType.Entity, e.EntityID);
		}
		
		#endregion
	}
}
