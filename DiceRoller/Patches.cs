// shamelessly based on ToyBox https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/8, which is under the MIT License

using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker;
//using Kingmaker.Controllers.GlobalMap;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
//using Kingmaker.UI._ConsoleUI.Models;
//using Kingmaker.UI.RestCamp;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using System;
using System.Collections.Generic;
//using Kingmaker.UI._ConsoleUI.GroupChanger;
using UnityModManager = UnityModManagerNet.UnityModManager;

namespace DiceRollerWotR.Patch
{
    internal static class NewChar
    {
        //public static Settings settings = Main.settings;
        public static LevelUpState levelUpStateData;

        [HarmonyPatch(typeof(LevelUpState), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(UnitEntityData), typeof(LevelUpState.CharBuildMode), typeof(bool) })]
        public static class LevelUpState_Patch
        {
            [HarmonyPriority(Priority.High)]
            public static void Postfix(UnitEntityData unit, LevelUpState.CharBuildMode mode, bool isPregen,  ref LevelUpState __instance)
            {
                if (Main.isActive())
                {
                    if (__instance.IsFirstCharacterLevel)
                    {
                        if (!__instance.IsPregen)
                        {
                            
                            foreach (StatType statType in StatTypeHelper.Attributes)
                            {
                                int i = RolledArray.stats[statType];
                                unit.Stats.GetStat(statType).BaseValue = i ;
                                
                            }
                            levelUpStateData = __instance;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LevelUpState), nameof(StatsDistribution.IsComplete))]
        public static class LevelUpState_IsComplete_Patch
        {
            public static void Postfix(ref LevelUpState __instance, ref bool __result)
            {
                if(__result && __instance == levelUpStateData)
                {
                    levelUpStateData = null;
                }
            }
        }

        [HarmonyPatch(typeof(StatsDistribution), nameof(StatsDistribution.Start))]
        public static class StatsDistribution_Start_Patch
        {
            [HarmonyPriority(Priority.High)]
            public static bool Prefix(StatsDistribution __instance, int pointCount)
            {
                if (Main.isActive())
                {
#if DEBUG
                    try
                    {
#endif
                        Traverse.Create(__instance).Property("Available").SetValue(true);
                        Traverse.Create(__instance).Property("Points").SetValue(RolledArray.stats.GetStatsSum());
                        Traverse.Create(__instance).Property("TotalPoints").SetValue( RolledArray.stats.GetStatsSum());
                        Traverse.Create(__instance).Property("StatValues").SetValue( RolledArray.stats.GetStats());
#if DEBUG
                    }
                    catch (Exception e)
                    {
                        Log.Write(e.ToString());
                    }
#endif

                    return false;
                }
                else
                    return true;
            }
        }
        [HarmonyPatch(typeof(StatsDistribution), nameof(StatsDistribution.CanRemove))]
        public static class StatsDistribution_CanRemove_Patch
        {
            [HarmonyPriority(Priority.High)]
            public static bool Prefix(ref bool __result, StatType attribute, StatsDistribution __instance)
            {
                if (Main.isActive())
                {
                    /*
                    int i = StatTypeHelper.Attributes.TryGetIndex(attribute);
                    if (i >= 0 && DiceRollerStatsDistribution.stats.)
                    */
                    __result = true;
                    return false;
                }
                else
                    return true;
            }
        }

        [HarmonyPatch(typeof(StatsDistribution), nameof(StatsDistribution.CanAdd))]
        public static class StatsDistribution_CanAdd_Patch
        {
            [HarmonyPriority(Priority.High)]
            public static bool Prefix(ref bool __result, StatType attribute, StatsDistribution __instance)
            {
                if (Main.isActive())
                {
                    __result = true;
                    return false;
                }
                else
                    return true;
            }
        }
    }

    [HarmonyPatch(typeof(StatsDistribution), nameof(StatsDistribution.GetAddCost))]
    public static class StatsDistribution_GetAddCost_Patch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(StatsDistribution __instance, StatType attribute, int __result)
        {
            if (Main.isActive())
            {
                __result = 0;
                return false;

            }
            else
                return true;
        }
    }


    [HarmonyPatch(typeof(StatsDistribution), nameof(StatsDistribution.GetRemoveCost))]
    public static class StatsDistribution_GetRemoveCost_Patch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(StatsDistribution __instance, StatType attribute, int __result)
        {
            if (Main.isActive())
            {
                __result = 0;
                return false;

            }
            else
                return true;
        }
    }

    [HarmonyPatch(typeof(StatsDistribution), nameof(StatsDistribution.Add))]
    public static class StatsDistribution_Add_Patch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(StatsDistribution __instance, UnitDescriptor unit, StatType attribute)
        {
            if (Main.isActive())
            {
                    StatType next = Helpers.GetNextAttribute(attribute).Value;
                    Log.Write($"Moving Down: {attribute} and {next} needs to be exchanged");
                    unit.Stats.Switch(attribute, next);
                    foreach (StatType statType in StatTypeHelper.Attributes)
                    {
                        __instance.StatValues[statType] = unit.Stats.GetStat(statType).BaseValue;
                    }

                //DiceRollerStatsDistribution.stats.Switch(attribute, next);
                //Traverse.Create(__instance).Property("StatValues").SetValue(RolledArray.stats.GetStats());
                return false;

            }
            else 
                return true;
        }
    }

    [HarmonyPatch(typeof(StatsDistribution), nameof(StatsDistribution.Remove))]
    public static class StatsDistribution_Remove_Patch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(StatsDistribution __instance, UnitDescriptor unit, StatType attribute)
        {
            if (Main.isActive())
            {
                /*
                if (__instance.CanAdd(attribute))
                {
                    int diff = DiceRollerStatsDistribution.stats[attribute];
                    DiceRollerStatsDistribution.stats.MoveDown(attribute);
                    diff -= DiceRollerStatsDistribution.stats[attribute];
                    unit.Stats.GetStat(attribute).BaseValue -= diff;
                    unit.Stats.GetStat(Helpers.GetNextAttribute(attribute).Value).BaseValue += diff;
                    AccessTools.Property(__instance.GetType(), "StatValues").SetValue(__instance, DiceRollerStatsDistribution.stats.GetStats());

                } */
                    StatType prev = Helpers.GetPreviousAttribute(attribute).Value;
                    Log.Write($"Moving Up: {attribute} and {prev} needs to be exchanged");
                    unit.Stats.Switch(attribute, prev);
                    foreach (StatType statType in StatTypeHelper.Attributes)
                    {
                        __instance.StatValues[statType] = unit.Stats.GetStat(statType).BaseValue;
                    }

                //DiceRollerStatsDistribution.stats.Switch(attribute, prev);
                //Traverse.Create(__instance).Property("StatValues").SetValue(RolledArray.stats.GetStats());

                return false;

            }
            else
                return true;
        }
    }

    [HarmonyPatch(typeof(StatsDistribution), nameof(StatsDistribution.IsComplete))]
    public static class StatsDistribution_IsComplete_Patch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(StatsDistribution __instance, ref bool __result)
        {
            if (Main.isActive())
            {
                
                    __result = true;
                
                return false;

            }
            else
                return true;
        }
    }

}

