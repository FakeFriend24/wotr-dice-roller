using DiceRollerWotR.StatArrayCalculation;
using HarmonyLib;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Class.LevelUp;

namespace DiceRollerWotR
{
    public class RolledArray
    {
        public static StatArray stats;

         
        public static void Reroll(Stat.StatArrayType statArrayType)
        {
            stats = new StatArray(statArrayType);
            if(Patch.NewChar.levelUpStateData != null)
            {
                foreach (StatType statType in StatTypeHelper.Attributes)
                {
                    int i = stats[statType];
                    Patch.NewChar.levelUpStateData.Unit.Stats.GetStat(statType).BaseValue = i;
                    
                    Traverse.Create(Patch.NewChar.levelUpStateData.StatsDistribution)
                            .Property("Points").SetValue(RolledArray.stats.GetStatsSum());
                    Traverse.Create(Patch.NewChar.levelUpStateData.StatsDistribution)
                            .Property("TotalPoints").SetValue(RolledArray.stats.GetStatsSum());
                    Traverse.Create(Patch.NewChar.levelUpStateData.StatsDistribution)
                            .Property("StatValues").SetValue(RolledArray.stats.GetStats());

                }

            }
        }


    }
}