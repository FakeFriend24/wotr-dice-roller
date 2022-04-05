using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiceRollerWotR
{
    class DiceBucket
    {
        public static readonly string REGEXFULL = @"[0-9]+d\[[0-9]+(,[0-9]+)?\](r\[[0-9]+(,[0-9]+)*\])?(k[hl][0-9]+)?";

        public List<Dice> dices;

        public DiceBucket(int v)
        {
            dices = new List<Dice> { new Dice(v, v, v) };
        }

        public DiceBucket(int dices, int maxV, int minV = 1) 
        {
            this.dices = new List<Dice>();
            for(int i = 0; i < dices; i++)
            {
                this.dices.Add(new Dice(maxV, minV));
            }
        }



        public static DiceBucket ParseExpression(string exp)
        {
            DiceBucket bucket;
            if (!DiceExpressionIsValid(exp))
                return new DiceBucket(0);

            bucket = new DiceBucket(ExtractAmount(exp), ExtractMaxValue(exp), ExtractMinValue(exp));
            if (exp.Contains("r"))
                bucket.Reroll(ExtractRerollValues(exp));
            if (exp.Contains("k"))
                bucket.Keep(ExtractKeepingAmount(exp), exp.Contains("h") ? true : false);
            return bucket;
        }

        static bool DiceExpressionIsValid(string exp)
        {
            return exp.IsCompleteMatch(REGEXFULL);
        }

        static int ExtractAmount(string regex)
        {
#if DEBUG
            Log.Write("AmountValue is: " + regex.Remove(regex.IndexOf("d")));
#endif

            return int.Parse(regex.Remove(regex.IndexOf("d")));
        }

        static int ExtractMaxValue(string regex)
        {
            regex = regex.Substring(regex.IndexOf("d")+1, regex.IndexOf("]") +1 - (regex.IndexOf("d") + 1));
            int iMaxStart;
            if (regex.Contains(","))
                iMaxStart = regex.IndexOf(",") + 1;
            else
                iMaxStart = regex.IndexOf("[") + 1; // Bsp: 3d[8] -> [8] -> 8] ->
#if DEBUG
            Log.Write("MaxValue is: " + regex.Substring(iMaxStart, regex.IndexOf("]") - iMaxStart ));
#endif
            return int.Parse(regex.Substring(iMaxStart, regex.IndexOf("]")-iMaxStart));
        }

        static int ExtractMinValue(string regex)
        {
            regex = regex.Substring(regex.IndexOf("d") + 1, regex.IndexOf("]") + 1 - (regex.IndexOf("d") + 1));
            int iMinStart = regex.IndexOf("[")+1;

            if (regex.Contains(",")) // Bsp: 3d[1,8] -> [1,8] -> iMin: 1, iMax: 2  -1
                return int.Parse(regex.Substring(iMinStart, regex.IndexOf(",")  - iMinStart));
            else
                return 1;
            
        }

        static int[] ExtractRerollValues(string regex)
        { // 5d[6]r[1]
            regex = regex.Substring(regex.IndexOf("r") + 2);
            string[] val = regex.Substring(0, regex.IndexOf("]")).Split(',');
#if DEBUG
            Log.Write("RerollValues are: " + val.ToString());
#endif

            int[] r = new int[val.Length];
            for (int i = 0; i < val.Length; i++)
                r[i] = int.Parse(val[i]);

            return r;                
        }

        static int ExtractKeepingAmount(string regex)
        {
            return int.Parse(regex.Substring(regex.IndexOf("k") + 2));
        }

        void Reroll(int[] values)
        {

            for(int i  = 0; i < dices.Count; i++)
            {
                int failsave = 100;
                while(values.Any(x => dices[i].Value == x) && failsave > 0)
                {
                    dices[i].RollDice();
                    failsave--;
                }
            }
        }

        void Keep(int amount, bool highest)
        {
            dices.Sort();
            if (highest)
                dices.Reverse();
            if(amount < dices.Count)
                dices.RemoveRange(amount, dices.Count - amount);            
        }

        public int GetValue()
        {
            int a = 0;
            dices.ForEach(x => a = a + x.Value);
            return a;
        }

        bool Contains(IEnumerable<int> values)
        {
            foreach (int v in values)
            {
                if (dices.Any(x => x.Value == v))
                    return true;
            }
            return false;
        }

    }

}
