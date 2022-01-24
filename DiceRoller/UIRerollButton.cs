using DiceRollerWotR.Patch;
using HarmonyLib;
using Kingmaker;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._PCView.CharGen;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.AbilityScores;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.AbilityScores;
using Kingmaker.UnitLogic.Class.LevelUp;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.SelectableState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceRollerWotR.UI
{
    [HarmonyLib.HarmonyPatch(typeof(CharGenAbilityScoresDetailedPCView), "BindViewImplementation")]
    public static class CharGenAbilityScoresDetailedPCView_BindViewImplementation_Patch
    {
        public static void Postfix(CharGenAbilityScoresDetailedPCView __instance)
        {
            GameObject Selector = Traverse.Create(__instance).Field<Transform>("m_StatAllocatorsContainer").Value.gameObject;
            for (int i = 0; i < Selector.transform.childCount; i++)
            {
                if (Selector.transform.GetChild(i).name.StartsWith("DiceRoller"))
                {
                    if (RerollButton.Instance == Selector.transform.GetChild(i).GetComponent<OwlcatButton>())
                        RerollButton.Instance = null;
                    UnityEngine.Object.Destroy(Selector.transform.GetChild(i).gameObject);
                    Log.Write("Cleaned up trash.");
                }
            }
            if (Main.isActive() && NewChar.levelUpStateData != null && NewChar.levelUpStateData.StatsDistribution.Available && NewChar.levelUpStateData.IsFirstCharacterLevel)
            {
                Log.Write("Button should be added.");
                RerollButton.InitializeRerollButton(Selector.GetComponent<RectTransform>(), __instance);
            }
        }
    }

    class RerollButton
    {
        public static OwlcatButton Instance;


        internal static void InitializeRerollButton(RectTransform transform, CharGenAbilityScoresDetailedPCView instance)
        { /*
            RectTransform buttonRect = new GameObject("DiceRoller_RerollButton", new Type[]
            {
                typeof(RectTransform) 
            }).GetComponent<RectTransform>();
            */
            GameObject prefab = Traverse.Create(Helpers.CurrentCharGenPCView).Field<OwlcatButton>("m_NextButton").Value.gameObject;
            Log.Write("prefab found!");
            Instance = GameObject.Instantiate(prefab, transform).GetComponent<OwlcatButton>();
            Instance.gameObject.name = "DiceRoller_RerollButton";
            Log.Write("Button Instantiated!");
            Instance.gameObject.SetActive(true);
            Log.Write("Button activated!");
            Traverse.Create(Instance).Property<bool>("Interactable").Value = true;
            Log.Write("Button interactive!");
            Instance.GetComponentInChildren<TextMeshProUGUI>().text = "Reroll Stat Array";
            Traverse.Create(Instance).Field<Button.ButtonClickedEvent>("m_OnSingleLeftClick").Value = new Button.ButtonClickedEvent();
            Log.Write("renew clicked Events!");
            Traverse.Create(Instance).Field<Button.ButtonClickedEvent>("m_OnSingleLeftClick").Value.AddListener(delegate ()
            {
                RolledArray.Reroll(Main.arrayType);                
                Log.Write("Reroll was Made!");
                CharGenAbilityScoresVM vm = Traverse.Create(instance).Property<CharGenAbilityScoresVM>("ViewModel").Value;
                if(vm != null) 
                {
                    foreach (CharGenAbilityScoreAllocatorVM charGenAbilityScoreAllocatorVM in vm.AbilityScoreAllocators)
                    {
                        ModifiableValue mv = NewChar.levelUpStateData.Unit.Stats.GetStat(charGenAbilityScoreAllocatorVM.StatType);
                        int v = RolledArray.stats[charGenAbilityScoreAllocatorVM.StatType];
                        mv.BaseValue = v;
                        charGenAbilityScoreAllocatorVM.UpdateStat(mv); 
                    }
                    Traverse.Create(instance).Method("OnPointsChanged", RolledArray.stats.GetPointBuy()).GetValue();
                    Log.Write("vm was updated.");
                }
                else
                {
                    Log.Write("vm was not found.");

                }
            });
            Log.Write("Button fully initialized!");

        }
    }
}
