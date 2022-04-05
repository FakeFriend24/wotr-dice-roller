using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiceRollerWotR
{

    internal static class ExtensionMethods
    {
        public static void Switch(this CharacterStats characterStats, StatType statType1, StatType statType2)
        {
#if DEBUG
            Log.Write($"Before: v1: {characterStats.GetStat(statType1).BaseValue} && v2: {characterStats.GetStat(statType2).BaseValue}");
#endif
            ModifiableValue stat1 = characterStats.GetStat(statType1);
            int val1 = stat1.BaseValue;
            ModifiableValue stat2 = characterStats.GetStat(statType2);
            int val2 = stat2.BaseValue;
            stat2.BaseValue = val1;
            stat1.BaseValue = val2;
#if DEBUG
            Log.Write($"After: v1: {characterStats.GetStat(statType1).BaseValue} && v2: {characterStats.GetStat(statType2).BaseValue}");
#endif
        }

        public static string StringJoin<T>(this IEnumerable<T> array, Func<T, string> map, string separator = " ") => string.Join(separator, array.Select(map));

#if DEBUG
        internal static readonly List<BlueprintScriptableObject> newAssets = new List<BlueprintScriptableObject>();
#endif

        public static T[] Sort<T>(this T[] array) where T : IComparable<T>
        {
            Array.Sort(array);
            return array;
        }

        public static bool IsCompleteMatch( this string s, string regex)
        {
            Match m = Regex.Match(s, regex);
            if(m.Success && m.Length == s.Length)
            {
                return true;
            }
            return false;
        }

        public static T[] Switch<T>(this T[] array, int oldIndex, int newIndex)
        {
            if (array.IndexIsSafe(oldIndex) && array.IndexIsSafe(newIndex))
            {
                T t = array[oldIndex];
                array[oldIndex] = array[newIndex];
                array[newIndex] = t;
            }
            return array;
        }

        public static Dictionary<T, U> Switch<T,U>(this Dictionary<T, U> dict, T firstKey, T secondKey)
        {
#if DEBUG
            Log.Write($"Before: key1: {dict[firstKey]} && key2: {dict[secondKey]}");
#endif

            if (dict.ContainsKey(firstKey) && dict.ContainsKey(secondKey))
            {

                U t = dict[firstKey];
                dict[firstKey] = dict[secondKey];
                dict[secondKey] = t;
            }
            #if DEBUG
                        Log.Write($"Before: key1: {dict[firstKey]} && key2: {dict[secondKey]}");
            #endif
            return dict;
        }



        public static int TryGetIndex<T>(this T[] array, T obj) 
        {
            for(int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(obj))
                    return i;
                
            }
            return -1;
        }

        public static bool IndexIsSafe(this Array array, int i)
        {
            return 0 <= i && i < array.Length;
        }

    }


}