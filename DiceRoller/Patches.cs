// shamelessly based on ToyBox https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/8, which is under the MIT License

using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker;
//using Kingmaker.Controllers.GlobalMap;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.AbilityScores;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.AbilityScores;
//using Kingmaker.UI._ConsoleUI.Models;
//using Kingmaker.UI.RestCamp;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
//using Kingmaker.UI._ConsoleUI.GroupChanger;
using UnityModManager = UnityModManagerNet.UnityModManager;

namespace DiceRollerWotR.Patch
{


    [HarmonyPatch(typeof(LevelUpState))]
    public static class LevelUpState_Patches
    {

        [HarmonyPostfix,
            HarmonyPatch(MethodType.Constructor),
            HarmonyPatch(new Type[] { typeof(UnitEntityData), typeof(LevelUpState.CharBuildMode), typeof(bool) })]
        [HarmonyPriority(Priority.VeryLow)]
        public static void _ctor(UnitEntityData unit, LevelUpState.CharBuildMode mode, bool isPregen, ref LevelUpState __instance)
        {
            if ((Main.isActive() || Main.isRespecActive) && __instance.IsFirstCharacterLevel && !__instance.IsPregen && !unit.IsPet && !__instance.IsLoreCompanion)
            { 

                foreach (StatType statType in StatTypeHelper.Attributes)
                {
                    int i = RolledArray.Stats[statType];
                    unit.Stats.GetStat(statType).BaseValue = i;

                }
            }
            Accessor.levelUpStateData = __instance; 
        }

    }

        [HarmonyPatch(typeof(StatsDistribution))]
        public static class StatsDistribution_Patches
        {

        [HarmonyPostfix, HarmonyPatch(nameof(StatsDistribution.IsComplete))]
        [HarmonyPriority(Priority.VeryLow)]
        public static void IsComplete(LevelUpState __instance, ref bool __result)
        {
            if (__result && __instance == Accessor.levelUpStateData)
            {
                Accessor.levelUpStateData = null;
            }
        }

        [HarmonyPostfix, HarmonyPatch(nameof(StatsDistribution.Start))]
        [HarmonyPriority(Priority.VeryLow)]
            public static void Start(StatsDistribution __instance, int pointCount)
            {
                if (Main.isActive() || Main.isRespecActive)
                {
#if DEBUG
                    try
                    {
#endif
                        Traverse.Create(__instance).Property("Available").SetValue(true);
                        Traverse.Create(__instance).Property("Points").SetValue(RolledArray.GetPointBuy());
                        Traverse.Create(__instance).Property("TotalPoints").SetValue( RolledArray.GetPointBuy());
                        foreach (StatType key in StatTypeHelper.Attributes)
                        {
                            __instance.StatValues[key] = RolledArray.Stats[key];
                        }
                        
#if DEBUG
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
#endif

                }
            }

        [HarmonyPostfix, HarmonyPatch(nameof(StatsDistribution.CanRemove))]
        [HarmonyPriority(Priority.VeryLow)]
            public static void CanRemove(ref bool __result, StatType attribute, StatsDistribution __instance)
            {
                if (Main.isActive())
                {
                    __result = false;
                    
                }
            }
        [HarmonyPostfix, HarmonyPatch(nameof(StatsDistribution.CanAdd))]
        [HarmonyPriority(Priority.VeryLow)]
        public static void CanAdd(ref bool __result, StatType attribute, StatsDistribution __instance)
        {
            if (Main.isActive())
            {
                __result = true;
            }
        }

        [HarmonyPrefix, HarmonyPatch(nameof(StatsDistribution.GetAddCost))]
        [HarmonyPriority(Priority.VeryHigh)]
        public static bool GetAddCost(StatsDistribution __instance, StatType attribute,ref int __result)
        {
            if (Main.isActive())
            {
                __result = 0;
                return false;
            }
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(nameof(StatsDistribution.GetRemoveCost))]
        [HarmonyPriority(Priority.VeryLow)]
        public static void GetRemoveCost(StatsDistribution __instance, StatType attribute, ref int __result)
        {
            if (Main.isActive())
            {
                __result = 0;

            }
        }

        [HarmonyPrefix, HarmonyPatch(nameof(StatsDistribution.Add))]
        [HarmonyPriority(Priority.High)]
        public static bool Add(StatsDistribution __instance, UnitDescriptor unit, StatType attribute)
        {
            if (Main.isActive())
            {
                    StatType next = Helpers.GetNextAttribute(attribute).Value;
#if DEBUG
                Log.Write($"Moving Down: {attribute} and {next} needs to be exchanged");
#endif                    
                    Dictionary<StatType, int> statValues = __instance.StatValues;
                    int num = statValues[attribute];
                    int num2 = statValues[next];
                    statValues[attribute] = num2;
                    statValues[next] = num;
                    ModifiableValue stat = unit.Stats.GetStat(attribute);
                    ModifiableValue stat2 = unit.Stats.GetStat(next);
                    num = stat.BaseValue;
                    num2 = stat2.BaseValue;
                    stat2.BaseValue = num;
                    stat.BaseValue = num2;

                //DiceRollerStatsDistribution.stats.Switch(attribute, next);
                //Traverse.Create(__instance).Property("StatValues").SetValue(RolledArray.stats.GetStats());
                return false;

            }
            else 
                return true;
        }
        [HarmonyPrefix, HarmonyPatch(nameof(StatsDistribution.Remove))]
        [HarmonyPriority(Priority.High)]
        public static bool Remove(StatsDistribution __instance, UnitDescriptor unit, StatType attribute)
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
#if DEBUG
                Log.Write($"Moving Up: {attribute} and {prev} needs to be exchanged");
                    unit.Stats.Switch(attribute, prev);
#endif
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
        [HarmonyPostfix, HarmonyPatch(nameof(StatsDistribution.IsComplete))]
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(StatsDistribution __instance, ref bool __result)
        {
            if (Main.isActive())
            { 
                
                    __result = true;

            }
        }
    }

}

