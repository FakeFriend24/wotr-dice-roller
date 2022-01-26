using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.CharGen;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Kingmaker.UI.MVVM._PCView.Common;
using Kingmaker.UI.MVVM._PCView.Common.MessageModal;
using Kingmaker.UnitLogic.Class.LevelUp;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DiceRollerWotR
{
    public static class Accessor
    {
        public static Transform StaticRoot
        {
            get
            {
                if (HasStaticCanvas)
                    return Game.Instance.UI.Canvas.transform;
                else
                    return Game.Instance.UI.Common.transform;
            }
        }
         
        public static Transform FadeCanvas
        {
            get
            {
                return Game.Instance.UI.FadeCanvas.transform;
            }
        }

        public static bool HasStaticCanvas
        {
            get => Game.Instance.UI.Canvas != null;
        }

        // Token: 0x17000024 RID: 36
        // (get) Token: 0x060000DA RID: 218 RVA: 0x00008A87 File Offset: 0x00006C87
        public static T Current<T> () where T : MonoBehaviour
        {
                T view = StaticRoot.GetComponentInDescendant<T>();
#if DEBUG
                if (view != null)
                {
                    Log.Write("Found Current (= first) of Type "+typeof(T).ToString());
                }
#endif
                return view;
            
        }

        public static GameObject NextButtonPrefab
        {
            get
            {
                GameObject prefab = Traverse.Create(Accessor.Current<CharGenPCView>()).Field<OwlcatButton>("m_NextButton").Value?.gameObject;
#if DEBUG
                if (prefab != null)
                {
                    Log.Write("Found NextButton-Prefab");
                }
#endif
                return prefab;
            }
        }

        public static GameObject YesButtonPrefab
        {
            get
            {
                GameObject prefab = Traverse.Create(FadeCanvas.GetComponentInDescendant<MessageModalPCView>()).Field<OwlcatButton>("m_DeclineButton").Value.gameObject;
#if DEBUG
                if (prefab != null)
                {
                    Log.Write("Found YesButton-Prefab");
                }
#endif
                return prefab;
            }
        }



        public static GameObject FeatureSearchPrefab
        {
            get
            {
                GameObject prefab = Accessor.Current<CharGenFeatureSearchPCView>()?.gameObject;
#if DEBUG
                if (prefab != null)
                {
                    Log.Write("Found FeatureSearch-Prefab");
                }
#endif
                return prefab;
            }
        }

        //public static Settings settings = Main.settings;
        public static LevelUpState levelUpStateData;
         
        public static Settings Settings;

        public static bool HasLevelUpStateData
        {
            get => levelUpStateData != null;
        }


    }
}
