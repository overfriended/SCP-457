using System;
using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Events;
using UnityEngine;

namespace SCP_457
{
	[PluginDetails(author = "Prince Frizzy", 
                   name = "SCP-457", 
                   description = "SCP-457 is the 'living' flame and takes a humanoid shape, he kills his enemies with the heat that he possesses.", 
                   id = "frizzy.scp457", 
                   version = "1.0.4", 
                   SmodMajor = 3, 
                   SmodMinor = 0,
                   SmodRevision = 0)]
	internal class SCP457 : Plugin
	{
		public override void OnDisable()
		{
			base.Info("Plugin SCP-457 is no longer active.");
		}

		public override void OnEnable()
		{
			base.Info("Plugin SCP-457 is now active.");
		}

		public override void Register()
		{
            base.AddEventHandlers(new EventLogic(this), Priority.Normal);
            base.AddCommand("spawnscp457", new SpawnSCP457Command(this));
			base.AddConfig(new ConfigSetting("scp457_spawnchance", 15, SettingType.NUMERIC, true, "The percent chance for SPC-457 to spawn."));
			base.AddConfig(new ConfigSetting("scp457_health", 500, SettingType.NUMERIC, true, "The amount of health points that SCP-457 has."));
            base.AddConfig(new ConfigSetting("scp457_min_health_heal", 0.35f, SettingType.FLOAT, true, "The minimum percent of health SCP-457 can randomly heal."));
            base.AddConfig(new ConfigSetting("scp457_max_health_heal", 0.65f, SettingType.FLOAT, true, "The maximum percent of health SCP-457 can randomly heal."));
			base.AddConfig(new ConfigSetting("scp457_baseradius", 3f, SettingType.FLOAT, true, "The base radius around SCP-457 that damages enemies."));
			base.AddConfig(new ConfigSetting("scp457_radiusstep", 1.5f, SettingType.FLOAT, true, "The amount the radius will increase every time SCP-457 hits an enemy."));
			base.AddConfig(new ConfigSetting("scp457_maxradius", 7f, SettingType.FLOAT, true, "The maximum radius around SCP-457 that damages enemies."));
			base.AddConfig(new ConfigSetting("scp457_radiusdecrease", 0.1f, SettingType.FLOAT, true, "The amount per second the SCP-457's damage radius will decrease."));
			base.AddConfig(new ConfigSetting("scp457_basedamage", 4f, SettingType.FLOAT, true, "The base damage per second the radius around SCP-457 will inflict on enemies."));
			base.AddConfig(new ConfigSetting("scp457_damagestep", 5f, SettingType.FLOAT, true, "The amount that the damage the radius around SCP-457 inflicts on enemies increases per hit."));
			base.AddConfig(new ConfigSetting("scp457_maxdamage", 30f, SettingType.FLOAT, true, "The max damage per second the radius around SCP-457 will inflict on enemies."));
			base.AddConfig(new ConfigSetting("scp457_damagedecrease", 0.2f, SettingType.FLOAT, true, "The amount that the damage the radius around SCP-457 inflicts on enemies decreases per second."));
            base.AddConfig(new ConfigSetting("scp457_tutorialallies", true, SettingType.BOOL, true, "If true, SCP-457 will not hurt tutorials. Useful for the Serpents Hand plugin."));
            base.AddConfig(new ConfigSetting("scp457_rank", new[] { "owner", "dev", "developer" }, SettingType.LIST, true, "Ranks allowed to spawn SCP-457."));
        }

        public void RefreshConfig()
        {
            RaRanks = GetConfigList("scp457_rank");
        }

        public static bool SteamIDIsSCP457(string id)
		{
			return SCP457.active457List.Contains(id);
		}

		public Player GetPlayerFromID(string id)
		{
			foreach (Player player in base.Server.GetPlayers(""))
			{
				if (id.Equals(player.SteamId))
				{
					return player;
				}
			}
			return null;
		}

		public bool RemoveSCP457(string id)
		{
			string text = null;
			foreach (string text2 in SCP457.active457List)
			{
				if (text2.Equals(id))
				{
					text = text2;
					this.GetPlayerFromID(text2).SetRank("white", " ", "");
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			SCP457.active457List.Remove(text);
			return true;
		}

		public static bool HasSCP457()
		{
			return SCP457.active457List.Count != 0;
		}

        public static string[] StringToStringArray(string input)
        {
            List<string> data = new List<string>();
            if (input.Length > 0)
            {
                string[] a = input.Split(' ');
                for (int i = 0; i < a.Count(); i++)
                {
                    data.Add(a[i]);
                }
            }
            return data.ToArray();
        }

        public SCP457()
		{
		}

		static SCP457()
		{
		}

		public static IDictionary<string, string> checkSteamIDforBadgeName = new Dictionary<string, string>();

		public static IDictionary<string, string> checkSteamIDforBadgeColor = new Dictionary<string, string>();

		public static IDictionary<Team, int> teamAliveCount = new Dictionary<Team, int>();

        public string[] RaRanks { get; private set; }

        public static List<string> active457List = new List<string>();
    }
}
