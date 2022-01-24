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
        }


    }
}