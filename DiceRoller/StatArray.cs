using Kingmaker.EntitySystem.Stats;
using System;
using System.Collections.Generic;
using static DiceRollerWotR.StatArrayCalculation.Stat;

namespace DiceRollerWotR.StatArrayCalculation
{
    public class StatArray : IComparable<StatArray> 
    { 
         
        public static Random generator = new Random(); 

        Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

        public StatArray(StatArrayType type)
        {
            for(int i = 0; i < StatTypeHelper.Attributes.Length; i++)
            {
                stats.Add(StatTypeHelper.Attributes[i], new Stat(type));
#if DEBUG
                Log.Write("A " + stats[StatTypeHelper.Attributes[i]].Value + " costs " + stats[StatTypeHelper.Attributes[i]].PointBuyValue + " Points.");
#endif
            }
        }




        public int GetPointBuy()
        {
            int sum = 0;
            foreach(KeyValuePair<StatType, Stat> keyValuePair in stats)
            {
                sum += keyValuePair.Value.PointBuyValue;
            }
            return sum;
        }

        public int GetStatsSum()
        {
            int sum = 0;
            foreach (KeyValuePair<StatType, Stat> keyValuePair in stats)
            {
                sum += keyValuePair.Value.Value;
            }
            return sum;
        }



        public int this[StatType statType]
        {
            get
            {
                if (stats.ContainsKey(statType))
                    return stats[statType].Value;
                else
                    return 0;
            }
        }

        public Dictionary<StatType, int> GetStats()
        {
            Dictionary<StatType, int> t = new Dictionary<StatType, int>();
            foreach(KeyValuePair<StatType,Stat> stat in stats)
            {
                t.Add(stat.Key, stat.Value.Value);
            }
            return t;
        }

        public void Switch(StatType statType1, StatType statType2)
        {
            Stat t1 = stats[statType1];
            Stat t2 = stats[statType2];
            stats[statType1] = t2;
            stats[statType2]= t1;

        }

        public void MoveUp(StatType index)
        {

            stats.Switch(index, Helpers.GetPreviousAttribute(index).Value);
        }

        public void MoveDown(StatType index)
        {
            stats.Switch(index, Helpers.GetNextAttribute(index).Value);
        }
        public bool CanMoveUp(StatType index)
        {
            return true;
        }

        public bool CanMoveDown(StatType index)
        {
            return true;
        }

        public int CompareTo(StatArray other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            // The temperature comparison depends on the comparison of
            // the underlying Double values.
            return GetPointBuy().CompareTo(other.GetPointBuy());
        }

        // Define the is greater than operator.
        public static bool operator >(StatArray operand1, StatArray operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        // Define the is less than operator.
        public static bool operator <(StatArray operand1, StatArray operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(StatArray operand1, StatArray operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(StatArray operand1, StatArray operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
    } 
}