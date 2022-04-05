using HarmonyLib;
using Kingmaker;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.AbilityScores;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.AbilityScores;
using Kingmaker.UnitLogic.Class.LevelUp;
using System;
using System.Collections.Generic;
using UniRx;

namespace DiceRollerWotR
{
    public class RolledArray
    {
        public enum StatArrayType
        {
            ThreeDice,
            FourMinusLowest,
            FourRerollOnesMinusLowest,
            FourMinusHighest,
            AllFeaturesExample,
            ThreeDiceRerollFourOrHigher,
            UseBrackets,
            TwoPlusSix,
            OneDTwenty,
            FiveDFour,
            NoTimeToWaste,
        }


        public static Dictionary<StatType, int> Stats;
         
        /*
        public static void Reroll(StatArrayType statArrayType)
        {
            StatArray newStats = new StatArray(statArrayType);
            Stats = newStats.GetStats();
            UpdateStatDistribution();
            //UpdateAllocatorVM();
        }
        */

        public static void Reroll(string regex)
        {
            
            Stats = new Dictionary<StatType, int>();
            for (int i = 0; i < StatTypeHelper.Attributes.Length; i++)
            {

                try
                {
                    Stats.Add(StatTypeHelper.Attributes[i], Parser.Parse(regex));
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Stats.Add(StatTypeHelper.Attributes[i], 10);

                }
                //                Log.Write("A " + stats[StatTypeHelper.Attributes[i]].Value + " costs " + stats[StatTypeHelper.Attributes[i]].PointBuyValue + " Points.");


            }
            UpdateStatDistribution();
        }


        public static int GetPointBuy()
        {
            int sum = 0;
            foreach (KeyValuePair<StatType, int> keyValuePair in Stats)
            {
                sum += GetPointBuy(keyValuePair.Value);
            }
            return sum;
        }

        public static int GetPointBuy(int i )
        {
            int v = i;
            int s = 0;
            if (v > 10)
            {
                v -= 10;
                while (v != 0)
                {
                    s += Math.Max(1, (v - (v % 2)) / 2);
                    v--;
                }
            }
            else if (v < 10)
            {
                v -= 10;
                while (v != 0)
                {
                    s += ((v) + ((v) % 2)) / 2;
                    v++;
                }
            }
            return s;

        }


        public static void UpdateStatDistribution()
        {


            if (Game.Instance.LevelUpController != null)
            {

                foreach (StatType statType in StatTypeHelper.Attributes)
                {
                    Accessor.levelUpStateData.Unit.Stats.GetStat(statType).BaseValue = RolledArray.Stats[statType];
                    Accessor.levelUpStateData.StatsDistribution.StatValues[statType] = Accessor.levelUpStateData.Unit.Stats.GetStat(statType).BaseValue;
                }

                Traverse.Create(Game.Instance.LevelUpController).Field<bool>("m_RecalculatePreview").Value = true;
                Traverse.Create(Game.Instance.LevelUpController).Method("UpdatePreview").GetValue();
            }
        }



    }
}