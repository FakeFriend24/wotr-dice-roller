using HarmonyLib;
using Kingmaker.EntitySystem.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using static DiceRollerWotR.RolledArray;

namespace DiceRollerWotR
{
    public class Settings : UnityModManager.ModSettings
    {
        public bool SaveAfterEveryChange = false;

        public bool KeepValuesForPointBuy = false;

        public string DiceExpression = @"4d[6]kh3";

        public int[] SavedArray;
         
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }

        public void DrawGUI(UnityModManager.ModEntry modEntry)
        {
            var fixedWidth = new GUILayoutOption[1] { GUILayout.ExpandWidth(false) };
            // prevent Changing Values when in Character Stat Screen.
            GUILayout.BeginHorizontal();
//            Accessor.Settings.KeepValuesForPointBuy = GUILayout.Toggle(Accessor.Settings.KeepValuesForPointBuy, GUIContent.none, fixedWidth);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b><color=cyan>Current Rolled Stats: </color></b>", fixedWidth);
#if DEBUG
            Log.Write("GUI: Works till here");
#endif

            for (int i = 0; i < StatTypeHelper.Attributes.Length; i++)
            {
                GUILayout.Label("<b> #" + i + "</b>: " + RolledArray.Stats[StatTypeHelper.Attributes[i]], fixedWidth);

            }
#if DEBUG
            Log.Write("GUI: Does this Work too?");
#endif

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reroll", fixedWidth))
            {
                RolledArray.Reroll(DiceExpression);
            }
            if (GUILayout.Button("Save", fixedWidth))
            {
                SaveCurrentArray();
            }
            GUILayout.BeginVertical();
            DiceExpression = GUILayout.TextField(DiceExpression,fixedWidth);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b><color=cyan>Saved Stats: </color></b>", fixedWidth);
            if (SavedArray != null)
                for (int i = 0; i < StatTypeHelper.Attributes.Length; i++)
                {
                    GUILayout.Label("<b>" + StatTypeHelper.Attributes[i].ToString() + "</b>: " + SavedArray[i], fixedWidth);
                }
            else
                GUILayout.Label("<b><color=red>None </color></b>", fixedWidth);

            GUILayout.EndHorizontal();
        }

        internal void LoadCurrentArray()
        {
            if (Accessor.Settings.SavedArray != null)
            {
                for (int i = 0; i < Accessor.Settings.SavedArray.Length; i++)
                    RolledArray.Stats[StatTypeHelper.Attributes[i]] = Accessor.Settings.SavedArray[i];
                RolledArray.UpdateStatDistribution();
            }
        }

        public void SaveCurrentArray()
        {
            if (SavedArray == null)
                SavedArray = new int[6];
            for (int i = 0; i < StatTypeHelper.Attributes.Length; i++)
            {
                SavedArray[i] = RolledArray.Stats[StatTypeHelper.Attributes[i]];
            }

        }
    }
}
