namespace SharedExp
{
    public class HardewValleyConfigInfo
    {
        public bool ExpNotification = false;
        public bool LevelNotification = false;
        public double GeneralExperienceFactor;
        public double FarmingExperienceFactor;
        public double FishingExperienceFactor;
        public double ForagingExperienceFactor;
        public double MiningExperienceFactor;
        public double CombatExperienceFactor;

        public void SetConfigInfo(bool expNotification,bool levelNotification, double generalExperienceFactor, double farmingExperienceFactor,
                                double fishingExperienceFactor, double foragingExperienceFactor, double miningExperienceFactor, double combatExperienceFactor)
        {
            ExpNotification = expNotification;
            LevelNotification = levelNotification;
            GeneralExperienceFactor = generalExperienceFactor;
            FarmingExperienceFactor = farmingExperienceFactor;
            FishingExperienceFactor = fishingExperienceFactor;
            ForagingExperienceFactor = foragingExperienceFactor;
            MiningExperienceFactor = miningExperienceFactor;
            CombatExperienceFactor = combatExperienceFactor;

        }
    }
    
}
