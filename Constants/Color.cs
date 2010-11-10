using System;

namespace SpacecraftGT
{
	public static class Color
	{
		public const string Signal = "\x00A7"; // section symbol §
		
		public const string Black = Signal + "0";
		public const string DarkBlue = Signal + "1";
		public const string DarkGreen = Signal + "2";
		public const string DarkTeal = Signal + "3";
		public const string DarkRed = Signal + "4";
		public const string Purple = Signal + "5";
		public const string DarkYellow = Signal + "6";
		public const string Gray = Signal + "7";
		public const string DarkGray = Signal + "8";
		public const string Blue = Signal + "9";
		public const string Green = Signal + "a";
		public const string Teal = Signal + "b";
		public const string Red = Signal + "c";
		public const string Pink = Signal + "d";
		public const string Yellow = Signal + "e";
		public const string White = Signal + "f";

		public const string Announce = Yellow;
		public const string PrivateMsg = Purple;
		public const string CommandResult = Teal;
		public const string CommandError = DarkRed;
	}
}