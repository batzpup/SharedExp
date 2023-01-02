using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;

namespace SharedExp
{
    
    public static class BatzpupExtensions
    {
        public static Farmer GetHostFarmer()
        {
            return Game1.MasterPlayer;
        }
        public static Farmer GetFarmerFromPeer(this IMultiplayerHelper helper, IMultiplayerPeer peer)
        {
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (farmer.UniqueMultiplayerID == peer.PlayerID)
                {
                    return farmer;
                }
                
            }
            return null;
        }
        public static void GainExp(this Farmer farmer, int which, int howMuch,IMonitor Monitor,IModHelper Helper)
        {
            //Allow for int 5 to allow luck skill to be more compatible
            if (howMuch <= 0)
            {
                return;
            }
            Monitor.Log($"Farmer {farmer.Name} gained {howMuch.ToString()} {(SkillNames)which} (XP Custom Gain). \n", LogLevel.Trace);
            if (!farmer.IsLocalPlayer)
            {
                farmer.queueMessage(17, Game1.player, new object[]
                {
                    which,
                    howMuch
                });
                return;
            }
            int newLevel = Farmer.checkForLevelGain(farmer.experiencePoints[which], farmer.experiencePoints[which] + howMuch);
            NetArray<int, NetInt> netArray = farmer.experiencePoints;
            netArray[which] += howMuch;
            int oldLevel = -1;
            if (newLevel != -1)
            {
                switch (which)
                {
                    case 0:
                        oldLevel = farmer.farmingLevel.Value;
                        farmer.farmingLevel.Value = newLevel;
                        break;
                    case 1:
                        oldLevel = farmer.fishingLevel.Value;
                        farmer.fishingLevel.Value = newLevel;
                        break;
                    case 2:
                        oldLevel = farmer.foragingLevel.Value;
                        farmer.foragingLevel.Value = newLevel;
                        break;
                    case 3:
                        oldLevel = farmer.miningLevel.Value;
                        farmer.miningLevel.Value = newLevel;
                        break;
                    case 4:
                        oldLevel = farmer.combatLevel.Value;
                        farmer.combatLevel.Value = newLevel;
                        break;
                    case 5:
                        if (!Helper.ReadConfig<ModConfig>().UseLuck)
                        {
                            break;
                        }
                        else
                        {
                            oldLevel = farmer.luckLevel.Value;
                            farmer.luckLevel.Value = newLevel;
                        }
                        break;
                }
            }
            if (newLevel > oldLevel)
            {
                for (int i = oldLevel + 1; i <= newLevel; i++)
                {
                    farmer.newLevels.Add(new Point(which, i));
                }
            }
           
        }
    }
}
