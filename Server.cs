using System;

namespace SpacecraftGT
{
	public class Server
	{
		public Server()
		{
			port = Configuration.GetInt("port", 25565);
			maxplayers = Configuration.GetInt("max-players", 16);
			name = Configuration.Get("server-name", "Minecraft Server");
			motd = Configuration.Get("motd", "Powered by " + Color.Green + "Spacecraft");
		}
		
		public void Run()
		{
			Spacecraft.Log("Running.");
		}
	}
}
