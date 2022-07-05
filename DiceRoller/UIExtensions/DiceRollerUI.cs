using HarmonyLib;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.AbilityScores;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.AbilityScores;
using Owlcat.Runtime.UI.Controls.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DiceRollerWotR.RolledArray;

namespace DiceRollerWotR
{
    class DiceRollerUI
    {
        public GameObject RootGameObject;
        public OwlcatButton RerollButton;
        public OwlcatButton SaveButton;
        public OwlcatButton LoadButton;
        public SearchBar DropdownButton;

        public DiceRollerUI(Transform parent, string name = "DiceRoller_DiceRollerUI")
        {
            RootGameObject = new GameObject("DiceRoller_RootContainer");
            RootGameObject.transform.SetParent(parent, false);
            var t = RootGameObject.AddComponent<HorizontalLayoutGroup>();
            t.childAlignment = TextAnchor.MiddleCenter;
            t.childControlWidth = true;
            t.childForceExpandWidth = false;
            t.childControlHeight = false;
            t.childForceExpandHeight = false;
            t.spacing = 3;
             
            SaveButton = UIElements.InitializeButton(RootGameObject.GetComponent<RectTransform>(), 
                                                     "Save", Accessor.Settings.SaveCurrentArray, "DiceRoller_SaveButton");

            LoadButton = UIElements.InitializeButton(RootGameObject.GetComponent<RectTransform>(), 
                                                     "Load", Accessor.Settings.LoadCurrentArray, "DiceRoller_LoadButton");

            DropdownButton = new SearchBar(RootGameObject.transform, "Type Expression ..." , Accessor.Settings.DiceExpression, "DiceRoller_RerollTypeSelector", 
                                           Enum.GetNames(typeof(StatArrayType)), 0, 
                                           delegate (int i) {
#if DEBUG
                                               Log.Write("Selected is: " + i);
                                               Log.Write("This converts to: " + (StatArrayType)i);
#endif

                                               Parser.ChangeExpressionTo((StatArrayType)i);
                                               DropdownButton.InputField.text = Accessor.Settings.DiceExpression;
                                               // Accessor.Settings.ArrayType = (StatArrayType) Enum.GetValues(typeof(StatArrayType)).GetValue(i);
                                               // Log.Write("Diceroll mode changed.");
                                           }, delegate(string s) { Accessor.Settings.DiceExpression = DropdownButton.InputField.text; });

            RerollButton = UIElements.InitializeButton(RootGameObject.GetComponent<RectTransform>(), 
                                                       "Reroll", delegate ()
                                                                 {
                                                                     RolledArray.Reroll(Accessor.Settings.DiceExpression);
#if DEBUG
                                                                     Log.Write("Reroll Button works!");
#endif
                                                                 }, 
                                                       "DiceRoller_RerollButton");

        }

    }
}
