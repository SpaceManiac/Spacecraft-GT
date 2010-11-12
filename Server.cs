using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SpacecraftGT
{
	public class Server
	{
		public int Port;
		public bool Running;
		public string WorldName;
		public string Name;
		public string Motd;
		
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
			
			World = null;
			PlayerList = new List<Player>();
			_Listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
		}
		
		public void Run()
		{
			World = new Map(WorldName);
			if (!File.Exists(WorldName + "/level.dat")) {
				Spacecraft.Log("Generating world " + WorldName);
				World.Generate();
				World.ForceSave();
			}
			
			_Listener.Start();
			Spacecraft.Log("Listening on port " + Port);
			Running = true;
			
			Chunk c = World.GetChunk(0, 0);
			Spacecraft.Log("Block at 0, 96, 0: " + c.GetBlock(0, 96, 0));
			
			while (Running) {
				// Check for new connections
				while (_Listener.Pending()) {
					AcceptConnection(_Listener.AcceptTcpClient());
					//Running = false;
				}
				
				// Rest
				Thread.Sleep(10);
			}
			
			World.ForceSave();
		}
		
		// ====================
		// Private helpers.
		
		private void AcceptConnection(TcpClient client)
		{
			PlayerList.Add(new Player(client));
		}
	}
}
