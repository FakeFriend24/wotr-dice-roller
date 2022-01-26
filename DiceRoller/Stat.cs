using System;
using System.Collections.Generic;

namespace DiceRollerWotR.StatArrayCalculation
{
    public class Stat : IComparable<Stat>
    {
        public enum StatArrayType
        {
            ThreeDice,
            FourMinusLowest,
            FourRerollOnesMinusLowest,
            FourMinusHighest,
            TwoPlusSix,
            OneDTwenty,
            FiveDFour,
            NoTimeToWaste,
        }

        StatArrayType type;
        public Stat(StatArrayType statType)
        {
            type = statType;
            InitDice(statType);
        }

        public Dice[] dice;

        public int Value
        {
            get
            {
                int v = 0;
                foreach (Dice d in dice)
                {
                    v += d.Value;
                }
                return v;
            }
        }
        public int PointBuyValue
        {
            get
            {
                int v = Value;
                int s = 0;
                if (v > 10)
                {
                    v -= 10;
                    while (v != 0)
                    {
                        s += Math.Max(1,(v - (v % 2)) / 2);
                        v--;
                    }
                }
                else if(v < 10)
                {
                    v -= 10;
                    while(v!= 0) { 
                        s += ((v) + ((v) % 2)) / 2;
                        v++;
                    } 
                }
                return s;
            }
        }

        public int this[int index]
        {
            get => dice[index].Value;
        }
        public int Length
        {
            get => dice.Length;
        }

        public void Reroll()
        {
            InitDice(type);
        }

        protected virtual void InitDice(StatArrayType statType)
        {
            List<Dice> l = new List<Dice>();
            switch (statType)
            {
                case StatArrayType.TwoPlusSix:
                    l.AddRange(RollDice(2, 1, 6));
                    l.Add(new Dice(6, 1, 6));
                    break;
                case StatArrayType.FourMinusLowest:
                    l.AddRange(RollDice(4, 1, 6));
                    l.Sort();
                    l.RemoveAt(0);
                    break;
                case StatArrayType.FourMinusHighest:
                    l.AddRange(RollDice(4, 1, 6));
                    l.Sort();
                    l.RemoveAt(l.Count - 1);
                    break;
                case StatArrayType.FourRerollOnesMinusLowest:
                    l.AddRange(RollDice(4, 1, 6));
                    while(l.Exists( d => d.Value == 1 ))
                    {
                        foreach(Dice d in l.FindAll(d => d.Value == 1))
                        {
                            d.RollDice();
                        }
                    }
                    l.Sort();
                    l.RemoveAt(0);
                    break;
                case StatArrayType.ThreeDice:
                    l.AddRange(RollDice(3, 1, 6));
                    l.Sort();
                    break;
                case StatArrayType.OneDTwenty:
                    l.AddRange(RollDice(1, 1, 20));
                    l.Sort();
                    break;
                case StatArrayType.FiveDFour:
                    l.AddRange(RollDice(5, 1, 4));
                    l.Sort();
                    break;
                case StatArrayType.NoTimeToWaste:
                    l.AddRange(RollDice(1, 18, 20));
                    l.Sort();
                    break;

            }
            dice = l.ToArray();


        }

        protected static Dice[] RollDice(int amount, int minValue, int maxValue)
        {
            Dice[] dices = new Dice[amount];
            for (int i = 0; i < dices.Length; i++)
            {
                dices[i] = new Dice(minValue, maxValue);
            }
            return dices;
        }

        public int CompareTo(Stat other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            // The temperature comparison depends on the comparison of
            // the underlying Double values.
            return Value.CompareTo(other.Value);
        }

        // Define the is greater than operator.
        public static bool operator >(Stat operand1, Stat operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        // Define the is less than operator.
        public static bool operator <(Stat operand1, Stat operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(Stat operand1, Stat operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(Stat operand1, Stat operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
    }
}