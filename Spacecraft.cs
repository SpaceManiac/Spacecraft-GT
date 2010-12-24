using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace SpacecraftGT
{
	public static class Spacecraft
	{
		public const int ProtocolVersion = 6;
		
		// private static StreamWriter _Log;
		public static Random Random;
		public static Server Server;
		
		public static void Main(string[] args)
		{
			// _Log = null;
			Log("Spacecraft is starting...");
			
			//Configuration.Load();
			
			if (Configuration.Defined("random-seed")) {
				Random = new Random(Configuration.GetInt("random-seed", 0));
			} else {
				Random = new Random();
			}
			
			Server = new Server();
			try {
				Server.Run();
			}
			catch (Exception e) {
				Log("Fatal uncaught exception: " + e);
			}
			
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
		
		public static string Base36Encode(long input)
		{
			if (input == 0) return "0";
			string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
			bool negative = (input < 0);
			StringBuilder sb = new StringBuilder();
			if (negative) {
				input = -input;
				sb.Append("-");
			}
			while (input > 0) {
				sb.Insert((negative ? 1 : 0), chars[(int)(input % 36)]);
				input /= 36;
			}
			return sb.ToString();
		}


	}
}
