using System;
using System.Net.Sockets;

namespace SpacecraftGT
{
	public class Player
	{
		private Connection _Conn;
		
		public Player(TcpClient client)
		{
			_Conn = new Connection(client);
		}
	}
}
