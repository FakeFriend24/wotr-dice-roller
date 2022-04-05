using System;

namespace DiceRollerWotR
{
    public class Dice : IComparable<Dice> 
    {
        int maxV;
        int minV;
        private int value;

        public int Value { get => value; private set => this.value = value; }

        public Dice(int maxValue, int minValue)
        {
            maxV = Math.Max(maxValue, minValue);
            minV = Math.Min(maxValue, minValue);
            this.RollDice();
        }

        public Dice(int value, int maxValue, int minValue)
        {
            maxV = Math.Max(maxValue, minValue);
            minV = Math.Min(maxValue, minValue);
            this.value = Math.Min(Math.Max(value, minV), maxV);
        }



        public void RollDice()
        {
            value = Main.randomGenerator.Next(minV, maxV+1);
        }

        public int CompareTo(Dice other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            // The temperature comparison depends on the comparison of
            // the underlying Double values.
            return value.CompareTo(other.value);
        }

        // Define the is greater than operator.
        public static bool operator >(Dice operand1, Dice operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        // Define the is less than operator.
        public static bool operator <(Dice operand1, Dice operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(Dice operand1, Dice operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(Dice operand1, Dice operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }

    }
}