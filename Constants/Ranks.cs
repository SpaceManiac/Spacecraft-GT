using System;
using System.Collections.Generic;

namespace SpacecraftGT {
	public enum Rank
	{
		Banned = -1,
		Guest,
		Builder,
		Mod,
		Admin
	}

	public static class RankInfo
	{
		private static Dictionary<Rank, string> colors;

		public static string RankColor(Rank rank)
		{
			if (colors.ContainsKey(rank)) {
				return colors[rank];
			} else {
				return Color.Black;
			}
		}
		
		public static bool IsOperator(Rank rank) {
			return (rank >= Rank.Mod);
		}

		static RankInfo()
		{
			colors = new Dictionary<Rank, string>();
			colors[Rank.Banned] = Color.Red;
			colors[Rank.Guest] = Color.White;
			colors[Rank.Builder] = Color.Green;
			colors[Rank.Mod] = Color.Yellow;
			colors[Rank.Admin] = Color.Blue;
		}
	}
}