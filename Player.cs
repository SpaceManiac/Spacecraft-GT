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
		
		public Chunk CurrentChunk;
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
			Spacecraft.Log(Username + " has joined!");
			//Spacecraft.Server.Spawn(this);
			Spawned = true;
			CurrentChunk = Spacecraft.Server.World.GetChunk(0, 0);
			X = Z = 0;
			Y = 96;
			UpdateChunks();
			_Conn.Transmit(PacketType.SpawnPosition, (int)X, (int)Y, (int)Z);
			// _Conn.Transmit(PacketType.PlayerPositionLook, X, Y, Y, Z, (float) 0, (float) 0, (byte) 1);
			_Conn.Transmit(PacketType.NamedEntitySpawn, EntityID, "Player", (int)X, (int)Y, (int)Z, (byte)0, (byte)0, (short)0);
		}
		
		public void UpdateChunks()
		{
			Chunk NewChunk = Spacecraft.Server.World.GetChunkAt((int)X, (int)Z);
			if (NewChunk != CurrentChunk) {
				List<Chunk> RemoveChunk = new List<Chunk>();
				List<Chunk> AddChunk = new List<Chunk>();
				foreach (Chunk c in Spacecraft.Server.World.GetChunksInRange(CurrentChunk)) {
					RemoveChunk.Add(c);
				}
				foreach (Chunk c in Spacecraft.Server.World.GetChunksInRange(NewChunk)) {
					if (RemoveChunk.Contains(c)) {
						RemoveChunk.Remove(c);
					} else {
						AddChunk.Add(c);
					}
				}
				
				foreach (Chunk c in RemoveChunk) {
					_Conn.Transmit(PacketType.PreChunk, c.ChunkX, c.ChunkZ, (byte) 0);
					foreach (Entity e in Spacecraft.Server.World.EntitiesIn(c)) {
						DespawnEntity(e);
					}
					VisibleChunks.Remove(c);
				}
				foreach (Chunk c in AddChunk) {
					_Conn.SendChunk(c);
					foreach (Entity e in Spacecraft.Server.World.EntitiesIn(c)) {
						SpawnEntity(e);
					}
					VisibleChunks.Add(c);
				}
				
				CurrentChunk = NewChunk;
			}	
		}
		
		public void DespawnEntity(Entity e)
		{
			_Conn.Transmit(PacketType.DestroyEntity, e.EntityID);
			VisibleEntities.Remove(e);
		}
		
		public void SpawnEntity(Entity e)
		{
			// TODO.
			VisibleEntities.Add(e);
		}
	}
}
