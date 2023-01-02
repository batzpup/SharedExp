using System;

using StardewModdingAPI;
using StardewValley;

namespace SharedExp
{
    public static class FarmerPatches
    {
        static IMonitor Monitor;
        static IModHelper Helper;
        // call this method from your Entry class
        public static void Initialize(IMonitor monitor,IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }
        public static void PreGainExperience(int which, ref int howMuch)
        { 
            
            if (Helper.ReadConfig<ModConfig>().SplitBetweenPlayers)
            {
                if(howMuch == 0)
                    return;
                int numberOfPlayers= Game1.getOnlineFarmers().Count;
                howMuch /= numberOfPlayers;
                if (howMuch == 0)
                    howMuch = 1;
                Monitor.Log($"Xp split amongst {numberOfPlayers} players", LogLevel.Trace);
            }
        }
        public static void PostGainExperience(int which, int howMuch)
        {
            try
            {
                if (howMuch <= 0)
                {
                    return;
                }

                if (!Helper.ReadConfig<ModConfig>().UseLuck && which == 5)
                {
                    return;
                }
                Monitor.Log($"Farmer {Game1.player.Name} gained {howMuch.ToString()} {(SkillNames)which} (XP PostGain)", LogLevel.Trace);
                Monitor.Log($"Farmer {Game1.player.Name} {(SkillNames)which} now has {Game1.player.experiencePoints[which]} xp(XP PostGain)", LogLevel.Trace);
                XpGainMessage message = new(which, howMuch);
                Helper.Multiplayer.SendMessage(message, "SharedExp.Batzpup.XPGainMessage", modIDs: new[] { "SharedExp.Batzpup" });
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(PostGainExperience)}:\n{ex}", LogLevel.Error);
            }
        }
    }
    public enum SkillNames
    {
        Farming = 0,
        Fishing = 1,
        Foraging = 2,
        Mining = 3,
        Combat =4,
        Luck = 5
    }
    
}
