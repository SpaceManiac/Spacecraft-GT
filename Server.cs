using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace SpacecraftGT
{
	public class Server
	{
		public int Port;
		public bool Running;
		public string WorldName;
		public string Name;
		public string Motd;
		public string ServerHash;
		
		public Map World;
		public List<Player> PlayerList;
		
		private TcpListener _Listener;
		
		public Server()
		{
			Port = Configuration.GetInt("port", 25565);
			Running = false;
			WorldName = Configuration.Get("world", "world");
			Name = Configuration.Get("server-name", "Minecraft Server");
			Motd = Configuration.Get("motd", "Powered by " + Color.Green + "Spacecraft");
			ServerHash = "-";
			
			World = null;
			PlayerList = new List<Player>();
			_Listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
		}
		
		public void Run()
		{
			
			World = new Map(WorldName);
			World.Time = 0;
			if (!File.Exists(WorldName + "/level.dat")) {
				Spacecraft.Log("Generating world " + WorldName);
				World.Generate();
				World.ForceSave();
			}
			
			_Listener.Start();
			Spacecraft.Log("Listening on port " + Port);
			Running = true;
			
			Stopwatch clock = new Stopwatch();
			clock.Start();
			double lastUpdate = 0;
			
			while (Running) {
				// Check for new connections
				while (_Listener.Pending()) {
					AcceptConnection(_Listener.AcceptTcpClient());
					//Running = false;
				}
				
				if (lastUpdate + 1 < clock.Elapsed.TotalSeconds) {
					foreach (Player p in PlayerList) {
						if (p.Spawned) p.Update();
					}
				}	
				
				// Rest
				Thread.Sleep(30);
			}
			
			World.ForceSave();
		}
		
		public void Spawn(Player player)
		{
			Spacecraft.Log(player.Username + " has joined");
			MessageAll(Color.Announce + player.Username + " has joined");
		}
		
		public void Despawn(Player player)
		{
			MessageAll(Color.Announce + player.Username + " has left");
			Spacecraft.Log(player.Username + " has left");
		}
		
		public void MessageAll(string message)
		{
			foreach(Player p in PlayerList) {
				p.SendMessage(message);
			}
		}
		
		// ====================
		// Private helpers.
		
		private void AcceptConnection(TcpClient client)
		{
			PlayerList.Add(new Player(client));
		}
	}
}
