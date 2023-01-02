using System;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace SharedExp
{
    public class ModEntry : Mod
    {
        public static ModConfig Config;
        public static bool IsHVLoaded = false;
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            FarmerPatches.Initialize(Monitor, helper);
            Harmony harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                prefix:new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.PreGainExperience)),
                postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.PostGainExperience))
            );
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnGameTicked;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
        }
        void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (Helper.ReadConfig<ModConfig>().ResetLuck)
            {
                Game1.player.experiencePoints[5] = 0;
                Game1.player.luckLevel.Value = 0;
            }
        }
        void OnGameTicked(object sender, UpdateTickedEventArgs updateTickedEventArgs)
        {
            if (Game1.player == Game1.MasterPlayer)
            {
                UpdateConfigMessage message = new(Config.SplitBetweenPlayers);
                Helper.Multiplayer.SendMessage(message, "SharedExp.Batzpup.UpdateConfigMessage", modIDs: new[] { "SharedExp.Batzpup" });
            }
        }
 
        void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            if(!Config.EnableCatchupExp)
                return;
            if(Game1.player != Game1.MasterPlayer)
                return;
            
            Farmer playerConnected = Helper.Multiplayer.GetFarmerFromPeer(e.Peer);
            if (Helper.ReadConfig<ModConfig>().ResetLuck)
            {
                playerConnected.experiencePoints[5] = 0;
                playerConnected.luckLevel.Value = 0;
            }
            for (int i = 0; i <  Game1.MasterPlayer.experiencePoints.Count; i++)
            {
                int difference = playerConnected.experiencePoints[i] - Game1.MasterPlayer.experiencePoints[i];
                if (difference >= 0)
                {
                    //Update Host
                    Game1.player.GainExp(i,difference,Monitor,Helper);
                }
                else
                {
                    //update client
                    XpGainMessage message = new(i, -difference);
                    Helper.Multiplayer.SendMessage(message, "SharedExp.Batzpup.XPGainMessage", modIDs: new[] { "SharedExp.Batzpup" },new []{playerConnected.UniqueMultiplayerID});
                }
                if (IsHVLoaded)
                {
                    IMod modEntryFor = Helper.GetModEntryFor("jahangmar.LevelingAdjustment");
                    Helper.Reflection.GetMethod(modEntryFor, "SetOldExpArray", true).Invoke(Array.Empty<object>());
                }
            }
        }
        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModManifest.UniqueID)
            {
                switch (@e.Type)
                {
                    case "SharedExp.Batzpup.XPGainMessage":
                        XpGainMessage xpGainMessage = e.ReadAs<XpGainMessage>();
                        Game1.player.GainExp(xpGainMessage.Which,xpGainMessage.HowMuch,Monitor,Helper);
                        if (IsHVLoaded)
                        {
                            IMod modEntry = Helper.GetModEntryFor("jahangmar.LevelingAdjustment");
                            Helper.Reflection.GetMethod(modEntry,"SetOldExpArray").Invoke();
                        }
                        break;
                    case "SharedExp.Batzpup.UpdateConfigMessage":
                        if (Game1.getFarmer(e.FromPlayerID).IsMainPlayer)
                        {
                            UpdateConfigMessage configMessage = e.ReadAs<UpdateConfigMessage>();
                            Config.SplitBetweenPlayers = configMessage.SplitExp;
                        }
                        break;
                }
            }
        }
        void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );
            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Split Exp between players",
                tooltip: () => "Divides xp gains by the number of players  (If one person is online, xp is normal, halved if 2 etc..)(Host Only)",
                getValue: () => Config.SplitBetweenPlayers,
                setValue: value => Config.SplitBetweenPlayers = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Catchup Exp",
                tooltip: () => "Allows players who join later to instantly get the same xp as the host, (Host Only)",
                getValue: () => Config.EnableCatchupExp,
                setValue: value => Config.EnableCatchupExp = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use Luck",
                tooltip: () => "Enables gaining luck exp as a skill ",
                getValue: () => Config.UseLuck,
                setValue: value => Config.UseLuck = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Reset Luck",
                tooltip: () => "When each day ends resets luck back to 0 ",
                getValue: () => Config.ResetLuck,
                setValue: value => Config.ResetLuck = value
            );
            if (Helper.ModRegistry.IsLoaded("jahangmar.LevelingAdjustment"))
            {
                IsHVLoaded = true;
            }
        }
    }
}

    

