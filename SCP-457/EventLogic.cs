using System;
using System.Collections.Generic;
using System.Threading;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using ServerMod2;
using ServerMod2.API;
using UnityEngine;

namespace SCP_457
{
    internal class EventLogic : EventArgs, IEventHandlerRoundStart, IEventHandlerCallCommand, IEventHandler, IEventHandlerWaitingForPlayers, IEventHandlerUpdate, IEventHandlerPlayerHurt, IEventHandlerPocketDimensionEnter, IEventHandler106Teleport, IEventHandler106CreatePortal, IEventHandlerSetRole, IEventHandlerPlayerDie, IEventHandlerSetRoleMaxHP, IEventHandlerContain106
	{
		public EventLogic(SCP457 plugin)
		{
			this.plugin = plugin;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			float num = (float)new System.Random().Next(0, 100);
			SCP457.active457List.Clear();
			if ((float)this.plugin.GetConfigInt("scp457_spawnchance") <= num)
			{
				return;
			}
            this.maxHealthHeal = this.plugin.GetConfigFloat("scp457_max_health_heal");
            this.minHealthHeal = this.plugin.GetConfigFloat("scp457_min_health_heal");
			this.damage = this.plugin.GetConfigFloat("scp457_basedamage");
			this.baseDamage = this.damage;
			this.damageStep = this.plugin.GetConfigFloat("scp457_damagestep");
			this.maxDamage = this.plugin.GetConfigFloat("scp457_maxdamage");
			this.damageDecrease = this.plugin.GetConfigFloat("scp457_damagedecrease");
			this.damageRadius = this.plugin.GetConfigFloat("scp457_baseradius");
			this.baseDamageRadius = this.damageRadius;
			this.damageRadiusStep = this.plugin.GetConfigFloat("scp457_radiusstep");
			this.maxDamageRadius = this.plugin.GetConfigFloat("scp457_maxradius");
			this.damageRadiusDecrease = this.plugin.GetConfigFloat("scp457_radiusdecrease");
			Player player = null;
			List<Player> list = new List<Player>();
			foreach (Player player2 in this.plugin.pluginManager.Server.GetPlayers(""))
			{
				if (player2.TeamRole.Role == Role.SCP_106)
				{
					player = player2;
					break;
				}
				if (player2.TeamRole.Team == Smod2.API.Team.SCP)
				{
					list.Add(player2);
				}
			}
			if (player == null)
			{
				player = list[new System.Random().Next(list.Count)];
			}
			player.SetRank("red", "SCP-457", "");
			SCP457.active457List.Add(player.SteamId);
			player.ChangeRole(Role.SCP_106, true, true, true, false);
		}

		public void OnUpdate(UpdateEvent e)
		{
			if (!SCP457.HasSCP457())
			{
				return;
			}
			if (DateTime.Now < this.lastCheckTime.AddSeconds(1.0))
			{
				return;
			}
			this.lastCheckTime = DateTime.Now;
			Player player = null;
			foreach (Player player2 in PluginManager.Manager.Server.GetPlayers(""))
			{
				if (SCP457.SteamIDIsSCP457(player2.SteamId))
				{
					player = player2;
					break;
				}
			}
			if (player == null)
			{
				return;
			}
            GameObject pObj = (GameObject)player.GetGameObject();
			Vector3 position = pObj.GetComponent<PlyMovementSync>().position;
			foreach (Player player3 in PluginManager.Manager.Server.GetPlayers(""))
			{
				if (player3.TeamRole.Team != Smod2.API.Team.SCP)
				{
                    if (player3.TeamRole.Team == Smod2.API.Team.TUTORIAL && this.plugin.GetConfigBool("scp457_tutorialallies"))
                    {
                        return;
                    }
                    GameObject pObj2 = (GameObject)player3.GetGameObject();
                    Vector3 position2 = pObj2.GetComponent<PlyMovementSync>().position;
                    if (Vector3.Distance(position, position2) <= this.damageRadius)
					{
						player3.Damage(checked((int)this.damage), DamageType.POCKET);
					}
				}
			}
			foreach (Player player4 in PluginManager.Manager.Server.GetPlayers(""))
			{
				if (player4.TeamRole.Team == Smod2.API.Team.SCP)
				{
                    if (player4.TeamRole.Team == Smod2.API.Team.TUTORIAL && this.plugin.GetConfigBool("scp457_tutorialallies"))
                    {
                        return;
                    }
                    GameObject pObj3 = (GameObject)player4.GetGameObject();
                    Vector3 position3 = pObj3.GetComponent<PlyMovementSync>().position;
                    if (Vector3.Distance(position, position3) <= this.damageRadius)
					{
                        Role role = player4.TeamRole.Role;
                        /*this.plugin.Debug("Performing SCP-457 health check.");
                        this.plugin.Debug(string.Concat(new object[]
                        {
                            "Player health is ", player4.GetHealth(), ", player max health is ", this.GetRoleMaxHealth(role), " and player SteamID is ", player4.SteamId
                        }));*/
						if (!SCP457.SteamIDIsSCP457(player4.SteamId) && player4.GetHealth() < this.GetRoleMaxHealth(role))
						{
                            this.plugin.Debug(string.Concat(new object[]
                            {
                                "Healing Player '", player4.Name, "'!"
                            }));
                            int damagePercentHealth = rng.Next((int)(this.GetRoleMaxHealth(role) * this.minHealthHeal), (int)(this.GetRoleMaxHealth(role) * this.maxHealthHeal));
                            int newHealth = Math.Min(this.GetRoleMaxHealth(role), player4.GetHealth() + damagePercentHealth);
                            player4.SetHealth(newHealth, DamageType.NUKE);
						}
					}
				}
			}
			float damageNum = (this.damage > this.baseDamage) ? (-this.damageDecrease) : this.damageDecrease;
			this.damage += damageNum;
			this.damage = ((Math.Sign(damageNum) == -1) ? Math.Max(this.baseDamage, this.damage) : Math.Min(this.baseDamage, this.damage));
			this.damage = Math.Max(1f, this.damage);
			float damageRadiusNum = (this.damageRadius > this.baseDamageRadius) ? (-this.damageRadiusDecrease) : this.damageRadiusDecrease;
			this.damageRadius += damageRadiusNum;
			this.damageRadius = ((Math.Sign(damageRadiusNum) == -1) ? Math.Max(this.baseDamageRadius, this.damageRadius) : Math.Min(this.baseDamageRadius, this.damageRadius));
			this.damageRadius = Math.Max(0f, this.damageRadius);
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!SCP457.SteamIDIsSCP457(ev.Attacker.SteamId) || SCP457.SteamIDIsSCP457(ev.Player.SteamId))
			{
				return;
			}
            if (ev.Player.TeamRole.Team == Smod2.API.Team.SCP || (ev.Player.TeamRole.Team == Smod2.API.Team.TUTORIAL && this.plugin.GetConfigBool("scp457_tutorialallies")))
            {
                return;
            }
            ev.Damage = 0f;
			this.damage += this.damageStep;
			this.damage = Math.Min(this.damage, this.maxDamage);
			this.damageRadius += this.damageRadiusStep;
			this.damageRadius = Math.Min(this.damageRadius, this.maxDamageRadius);
		}

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            plugin.RefreshConfig();
        }

        public void OnPocketDimensionEnter(PlayerPocketDimensionEnterEvent e)
		{
			if (!SCP457.HasSCP457())
			{
				return;
			}
			e.TargetPosition = e.LastPosition;
		}

		public void On106Teleport(Player106TeleportEvent e)
		{
			if (SCP457.SteamIDIsSCP457(e.Player.SteamId))
			{
				e.Position = e.Player.GetPosition();
			}
		}

		public void On106CreatePortal(Player106CreatePortalEvent e)
		{
            if (SCP457.SteamIDIsSCP457(e.Player.SteamId))
            {
                e.Position = new Vector(-100f, -100f, -100f);
            }
		}

		public void OnSetRole(PlayerSetRoleEvent e)
		{
			Player player = e.Player;
			if (SCP457.SteamIDIsSCP457(player.SteamId) && e.Role == Role.SCP_106)
			{
				player.SetHealth(this.plugin.GetConfigInt("scp457_health"), DamageType.NUKE);
				new Thread(new ThreadStart(this.DelayHealthSet)).Start();
			}
			if (SCP457.SteamIDIsSCP457(player.SteamId) && e.Role != Role.SCP_106)
			{
				this.plugin.RemoveSCP457(player.SteamId);
			}
		}

		private void DelayHealthSet()
		{
			Thread.Sleep(10000);
			foreach (string id in SCP457.active457List)
			{
				Player playerFromID = this.plugin.GetPlayerFromID(id);
				this.plugin.GetConfigInt("scp457_health");
				playerFromID.SetHealth(this.plugin.GetConfigInt("scp457_health"), DamageType.NUKE);
			}
		}

		public void OnPlayerDie(PlayerDeathEvent e)
		{
			Player player = e.Player;
			if (SCP457.SteamIDIsSCP457(player.SteamId))
			{
				this.plugin.RemoveSCP457(player.SteamId);
			}
		}

		public void OnSetRoleMaxHP(SetRoleMaxHPEvent e)
		{
			if (e.Role == Role.SCP_106 && SCP457.HasSCP457())
			{
				e.MaxHP = this.plugin.GetConfigInt("scp457_health");
				this.plugin.Info(string.Concat(new object[]
				{
					"Setting max health for SCP-457 to ", e.MaxHP, " for ", e.Role
				}));
			}
		}

		public void OnContain106(PlayerContain106Event e)
		{
			if (!SCP457.HasSCP457())
			{
				return;
			}
			e.ActivateContainment = false;
		}

        public void OnCallCommand(PlayerCallCommandEvent ev)
        {
            if (ev.Command.ToLower().StartsWith("spawnscp457"))
            {
                string[] a = new SpawnSCP457Command(plugin).OnCall(ev.Player, SCP457.StringToStringArray(ev.Command.Replace("spawnscp457 ", "")));
                string msg = "\n";
                for (int i = 0; i < a.Length; i++)
                {
                    msg += a[i];
                    if (i != a.Length - 1)
                    {
                        msg += Environment.NewLine;
                    }
                }
                ev.ReturnMessage = msg;
            }
        }

        private int GetRoleMaxHealth(Role role)
        {
            switch (role)
            {
                case Role.SCP_173:
                    return ConfigFile.GetInt("scp173_hp", 3200);
                case Role.CLASSD:
                    return ConfigFile.GetInt("classd_hp", 100);
                case Role.SCP_106:
                    return ConfigFile.GetInt("scp106_hp", 650);
                case Role.NTF_SCIENTIST:
                    return ConfigFile.GetInt("ntfscientists_hp", 120);
                case Role.SCP_049:
                    return ConfigFile.GetInt("scp049_hp", 1700);
                case Role.SCIENTIST:
                    return ConfigFile.GetInt("scientist_hp", 100);
                case Role.SCP_079:
                    return ConfigFile.GetInt("scp079_hp", 10000);
                case Role.CHAOS_INSURGENCY:
                    return ConfigFile.GetInt("ci_hp", 120);
                case Role.SCP_096:
                    return ConfigFile.GetInt("scp096_hp", 2000);
                case Role.SCP_049_2:
                    return ConfigFile.GetInt("scp049-2_hp", 400);
                case Role.NTF_LIEUTENANT:
                    return ConfigFile.GetInt("ntfl_hp", 120);
                case Role.NTF_COMMANDER:
                    return ConfigFile.GetInt("ntfc_hp", 150);
                case Role.NTF_CADET:
                    return ConfigFile.GetInt("ntfg_hp", 100);
                case Role.TUTORIAL:
                    return ConfigFile.GetInt("ntfc_hp", 150);
                case Role.FACILITY_GUARD:
                    return ConfigFile.GetInt("facilityguard_hp", 100);
                case Role.SCP_939_53:
                    return ConfigFile.GetInt("scp939_53_hp", 2200);
                case Role.SCP_939_89:
                    return ConfigFile.GetInt("scp939_89_hp", 2200);
            }
            return 100;
        }

        private SCP457 plugin;

		private float damageRadius;

		private float baseDamageRadius;

		private float damageRadiusStep;

		private float maxDamageRadius;

		private float damageRadiusDecrease;

		private float damage;

		private float baseDamage;

		private float damageStep;

		private float maxDamage;

		private float damageDecrease;
        
        private float maxHealthHeal;

        private float minHealthHeal;

		private DateTime lastCheckTime;

        static readonly private System.Random rng = new System.Random();
	}
}
