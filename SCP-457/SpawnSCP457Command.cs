using System;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace SCP_457 {
	internal class SpawnSCP457Command : ICommandHandler
	{
		public SpawnSCP457Command(SCP457 plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Spawn in someone as SCP-457.";
		}

		public string GetUsage()
		{
			return "spawnscp457 PlayerName/PlayerId";
		}

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (!(sender is Server) && sender is Player player && !plugin.RaRanks.Contains(player.GetRankName()))
            {
                return new[]
                {
                    $"You (rank {player.GetRankName() ?? "NULL"}) do not have permissions to run this command."
                };
            }
            if (args.Length == 0)
            {
                return new string[]
                {
                    this.GetUsage()
                };
            }
            player = GetPlayerFromString.GetPlayer(args[0]);
            if (player != null) {
                player.SetRank("red", "SCP-457", "");
                SCP457.active457List.Add(player.SteamId);
                player.ChangeRole(Role.SCP_106, true, true, true, false);
                return new string[]
                {
                    "Made " + player.Name + " SCP-457!"
                };
            }
			return new string[]
			{
				string.Format("{0} is not a valid player name or player id!", args[0])
			};
		}

		private readonly SCP457 plugin;
	}
    
	public class GetPlayerFromString
	{
		private static Server Server => PluginManager.Manager.Server;

		public static Player GetPlayer(string args)
		{
			Player playerOut = null;
			if (short.TryParse(args, out short pID))
			{
				foreach (Player pl in Server.GetPlayers())
					if (pl.PlayerId == pID)
						return pl;
			}
			else if (long.TryParse(args, out long sID))
			{
				foreach (Player pl in Server.GetPlayers())
					if (pl.SteamId == sID.ToString())
						return pl;
			}
			else
			{
				//Takes a string and finds the closest player from the playerlist
				int maxNameLength = 31, LastnameDifference = 31;
				string str1 = args.ToLower();
				foreach (Player pl in Server.GetPlayers(str1))
				{
					if (!pl.Name.ToLower().Contains(args.ToLower()))
						continue;
					if (str1.Length < maxNameLength)
					{
						int x = maxNameLength - str1.Length;
						int y = maxNameLength - pl.Name.Length;
						string str2 = pl.Name;
						for (int i = 0; i < x; i++)
						{
							str1 += "z";
						}
						for (int i = 0; i < y; i++)
						{
							str2 += "z";
						}
						int nameDifference = LevenshteinDistance.Compute(str1, str2);
						if (nameDifference < LastnameDifference)
						{
							LastnameDifference = nameDifference;
							playerOut = pl;
						}
					}
				}
			}
			return playerOut;
		}
	}
    
	internal static class LevenshteinDistance
	{
		/// <summary>
		/// Compute the distance between two <see cref="string"/>s.
		/// </summary>
		internal static int Compute(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			// Step 1
			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			// Step 2
			for (int i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (int j = 0; j <= m; d[0, j] = j++)
			{
			}

			// Step 3
			for (int i = 1; i <= n; i++)
			{
				//Step 4
				for (int j = 1; j <= m; j++)
				{
					// Step 5
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					// Step 6
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			// Step 7
			return d[n, m];
		}
	}
}
