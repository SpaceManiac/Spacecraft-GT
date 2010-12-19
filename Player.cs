using System;
using System.Collections.Generic;
using System.Net.Sockets;

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
		
		public void Spawn()
		{
			Spacecraft.Server.Spawn(this);
			Spawned = true;
			CurrentChunk = null;
			X = Spacecraft.Server.World.SpawnX + 0.5;
			Y = Spacecraft.Server.World.SpawnY + 5;
			Z = Spacecraft.Server.World.SpawnZ + 0.5;
			Update();
			_Conn.Transmit(PacketType.SpawnPosition, (int)X, (int)Y, (int)Z);
			_Conn.Transmit(PacketType.PlayerPositionLook, X, Y, Y, Z, (float) 0, (float) 0, (byte) 1);
			// _Conn.Transmit(PacketType.NamedEntitySpawn, EntityID, "Player", (int)X, (int)Y, (int)Z, (byte)0, (byte)0, (short)0);
		}
		
		public void Despawn()
		{
			Spacecraft.Server.Despawn(this);
			Spawned = false;
			CurrentChunk = null;
		}
		
		public void SendMessage(string message)
		{
			_Conn.Transmit(PacketType.Message, message);
		}
		
		public void RecvMessage(string message)
		{
			Spacecraft.Log("<" + Username + "> " + message);
			Spacecraft.Server.MessageAll("<" + Username + "> " + message);
		}
		
		public override void Update()
		{
			Chunk newChunk = Spacecraft.Server.World.GetChunkAt((int)X, (int)Z);
			
			if (newChunk != CurrentChunk) {
				List<Chunk> newVisibleChunks = new List<Chunk>();
				
				foreach (Chunk c in Spacecraft.Server.World.GetChunksInRange(newChunk)) {
					newVisibleChunks.Add(c);
				}
				foreach (Chunk c in VisibleChunks) {
					if (!newVisibleChunks.Contains(c)) {
						_Conn.Transmit(PacketType.PreChunk, c.ChunkX, c.ChunkZ, (byte) 0);
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
		
		private void DespawnEntity(Entity e)
		{
			if (e == this) return;
			_Conn.Transmit(PacketType.DestroyEntity, e.EntityID);
		}
		
		private void SpawnEntity(Entity e)
		{
			if (e == this) return;
			
			if (e is Player) {
				Player p = (Player) e;
				_Conn.Transmit(PacketType.NamedEntitySpawn, p.EntityID,
					p.Username, (int)p.X, (int)p.Y, (int)p.Z,
					(byte)0, (byte)0, (short)1);
			} else {
				SendMessage(Color.Purple + "Spawning " + e);
			}
		}
		
		override public string ToString()
		{
			return "[Entity.Player " + EntityID + ": " + Username + "]";
		}
		
	}
}
