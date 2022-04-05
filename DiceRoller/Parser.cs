using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiceRollerWotR
{
    public static class Parser
    {
        struct Strings {
            public string left;
            public string found;
            public string right;

            public Strings(string left, string found, string right)
            {
                this.left = left;
                this.found = found;
                this.right = right;
            }
        }

        public static int Parse(String exp)
        {
#if DEBUG
            Log.Write("Current String to Parse:" + exp);
#endif

            // Sanity Checks
            if (String.IsNullOrWhiteSpace(exp))
                return 0;
            exp = exp.ToLower().Replace(" ", "");
            // Check for Brackets
            if (DivideBy(exp, @"\((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)", out Strings strings))
            {
#if DEBUG
                Log.Write("-> Found Brackets: " + strings.found);
#endif
                return Parse(strings.left+Parse(strings.found.Substring(1, strings.found.Length - 2))+strings.right);
            }

            // CheckForDiceExp
            if (DivideBy(exp, DiceBucket.REGEXFULL, out strings))
            {
#if DEBUG
                Log.Write("-> Found DiceExp: " + strings.found);
#endif
                return Parse(strings.left + DiceBucket.ParseExpression(strings.found).GetValue() + strings.right);
            }

            if(DivideBy(exp, @"[0-9]+[\+\-\*][0-9]+", out strings))
            {
#if DEBUG
                Log.Write("-> Found Calculation:" + strings.found);
#endif
                return Parse(strings.left + CalcBaseExpression(strings.found) + strings.right);
            }

            if(exp.IsCompleteMatch(@"[0-9]+"))
            {
#if DEBUG
                Log.Write("-> Found Number: " + exp);
#endif

                return int.Parse(exp);
            }
#if DEBUG
            Log.Write("-> Found Crap: " + exp);
#endif

            return 0;
        }

        internal static void ChangeExpressionTo(RolledArray.StatArrayType statType)
        {
            switch(statType)
            {
                case RolledArray.StatArrayType.ThreeDice:
                    Accessor.Settings.DiceExpression = "3d[6]";
                    break;
                case RolledArray.StatArrayType.AllFeaturesExample:
                    Accessor.Settings.DiceExpression = "3*4d[2,6]r[4,5]kl(1d[4]-1)";
                    break;
                case RolledArray.StatArrayType.UseBrackets:
                    Accessor.Settings.DiceExpression = "5d[1,(1d[4]+1)]";
                    break;
                case RolledArray.StatArrayType.FiveDFour:
                    Accessor.Settings.DiceExpression = "5d[4]";
                    break;
                case RolledArray.StatArrayType.ThreeDiceRerollFourOrHigher:
                    Accessor.Settings.DiceExpression = "3d[6]r[4,5,6]";
                    break;
                case RolledArray.StatArrayType.FourMinusHighest:
                    Accessor.Settings.DiceExpression = "4d[6]kl3";
                    break;
                case RolledArray.StatArrayType.FourMinusLowest:
                    Accessor.Settings.DiceExpression = "4d[6]kh3";
                    break;
                case RolledArray.StatArrayType.OneDTwenty:
                    Accessor.Settings.DiceExpression = "1d[20]";
                    break;
                case RolledArray.StatArrayType.FourRerollOnesMinusLowest:
                    Accessor.Settings.DiceExpression = "4d[6]r[1]kh3";
                    break;
                case RolledArray.StatArrayType.TwoPlusSix:
                    Accessor.Settings.DiceExpression = "2d[6]+6";
                    break;
                case RolledArray.StatArrayType.NoTimeToWaste:
                    Accessor.Settings.DiceExpression = "1d[18,20]";
                    break;
                default:
                    break;
            }
        }

        static int CalcBaseExpression(string expression)
        {
            DivideBy(expression, @"[\+\-\*]", out Strings strings);
            int a = int.Parse(strings.left);
            int b = int.Parse(strings.right);
            switch (strings.found)
            {
                case "+":
                default:
                    return a + b;
                case "-":
                    return Math.Max(0,a - b);
                case "*":
                    return a * b;
            }
        }

        static bool DivideBy(string exp, string regEx, out Strings strings)
        {
            Match match = Regex.Match(exp, regEx);
            if(match.Success)
            {
                strings = new Strings(exp.Substring(0, match.Index), exp.Substring(match.Index, match.Length), exp.Substring(match.Index + match.Length));
                return true;
            } else
            {
                strings = new Strings(exp, null, null);
                return false;
            }
        }
        
    }
}
