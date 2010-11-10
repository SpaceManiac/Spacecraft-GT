using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SpacecraftGT
{
	public static class Spacecraft
	{
		// private static StreamWriter _Log;
		public static Random Random;
		public static Server Server;
		
		public static void Main(string[] args)
		{
			// _Log = null;
			Log("Spacecraft is starting...");
			
			Configuration.Load();
			
			if (Configuration.Defined("random-seed")) {
				Random = new Random(Configuration.GetInt("random-seed", 0));
			} else {
				Random = new Random();
			}
			
			
			Server = new Server();
			Server.Run();
			
			Log("Bye!");
		}
		
		public static void Log(string message)
		{
			// lock(_Log) {
				Console.WriteLine(FormatTime() + "    " + message);
			// }
		}
		
		public static string FormatTime()
		{
			return DateTime.Now.ToString("HH:mm:ss");
		}

		public static string MD5sum(string str)
		{
			MD5CryptoServiceProvider crypto = new MD5CryptoServiceProvider();
			byte[] data = Encoding.ASCII.GetBytes(str);
			data = crypto.ComputeHash(data);
			StringBuilder ret = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
				ret.Append(data[i].ToString("x2").ToLower());
			return ret.ToString();
		}
		
		public static string Base64Encode(string str)
		{
		   return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
		}
	}
}
