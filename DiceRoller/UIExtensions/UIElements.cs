using DiceRollerWotR.Patch;
using HarmonyLib;
using Kingmaker;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._PCView.CharGen;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.AbilityScores;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
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
using UnityEngine.Events;
using UnityEngine.UI;

namespace DiceRollerWotR
{
    [HarmonyLib.HarmonyPatch(typeof(CharGenAbilityScoresDetailedPCView), "BindViewImplementation")]
    public static class CharGenAbilityScoresDetailedPCView_BindViewImplementation_Patch
    {
        public static void Postfix(CharGenAbilityScoresDetailedPCView __instance)
        {
            Log.Write("Instantiate DiceRollUI.");
            GameObject Selector = Traverse.Create(__instance).Field<Transform>("m_StatAllocatorsContainer").Value.gameObject;
            for (int i = 0; i < Selector.transform.childCount; i++)
            {
                if (Selector.transform.GetChild(i).name.StartsWith("DiceRoller"))
                {
                    if (UIElements.RollerInstance.RootGameObject == Selector.transform.GetChild(i).gameObject)
                        UIElements.RollerInstance = null;
                    UnityEngine.Object.Destroy(Selector.transform.GetChild(i).gameObject);
#if DEBUG
                    Log.Write("Cleaned up trash.");
#endif
                }
            }
            UIElements.RollerInstance = null;
            if (Main.isActive() && Accessor.HasLevelUpStateData && Accessor.levelUpStateData.StatsDistribution.Available && Accessor.levelUpStateData.IsFirstCharacterLevel)
            {
 #if DEBUG
               Log.Write("RerollView should be added.");
 #endif
               UIElements.InitializeRerollView(Selector.GetComponent<RectTransform>(), __instance);
            }
        }
    }

    class UIElements
    {
        public static DiceRollerUI RollerInstance;
         
        internal static OwlcatButton InitializeButton(RectTransform parent, string text,  UnityAction onClick, String name = "DiceRoller_NewButton")
        { 
            OwlcatButton button = GameObject.Instantiate(Accessor.YesButtonPrefab, parent).GetComponent<OwlcatButton>();
            button.gameObject.name = name;
            //GameObject.Destroy(button.transform.Find("ArrowImage").gameObject);
#if DEBUG
            Log.Write("Button Instantiated!");
#endif
            button.gameObject.SetActive(true);
#if DEBUG
            Log.Write("Button activated!");
#endif
            button.Interactable = true;
#if DEBUG
            Log.Write("Button interactive!");
#endif
            button.GetComponentInChildren<TextMeshProUGUI>().text = text;
            button.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            Traverse.Create(button).Field<Button.ButtonClickedEvent>("m_OnSingleLeftClick").Value = new Button.ButtonClickedEvent();
#if DEBUG
            Log.Write("renew clicked Events!");
#endif
            Traverse.Create(button).Field<Button.ButtonClickedEvent>("m_OnSingleLeftClick").Value.AddListener(onClick);
#if DEBUG
            Log.Write("Button fully initialized!");
#endif
            button.gameObject.GetComponent<LayoutElement>().flexibleWidth = 1;
            button.gameObject.GetComponent<LayoutElement>().minWidth = 0;
            button.gameObject.GetComponent<LayoutElement>().preferredWidth = 0;
            return button;
        }

        internal static void InitializeRerollView(RectTransform transform, CharGenAbilityScoresDetailedPCView instance)
        {
            if(Main.isActive() && RollerInstance == null && ((CharGenAbilityScoresVM)instance.GetViewModel()).IsPointBuyMode)
            {
                RollerInstance = new DiceRollerUI(transform);
            }
        }
    }
}
