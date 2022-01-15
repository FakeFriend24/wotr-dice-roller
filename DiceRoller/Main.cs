// Copyright (c) 2019 v1ld.git@gmail.com
// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using System; 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Root.Strings.GameLog;
using Kingmaker.GameModes;
using Kingmaker.Globalmap;
using Kingmaker.Globalmap.Blueprints;
using Kingmaker.Globalmap.View;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.ServiceWindow.LocalMap;
using Kingmaker.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityModManagerNet;
using Owlcat.Runtime.Core;
using Owlcat.Runtime.Core.Logging;
using Kingmaker.UI;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.LocalMap;
using Kingmaker.UI.MVVM._VM.ServiceWindows.LocalMap;
using HarmonyLib;
using Kingmaker.EntitySystem.Stats;
using static DiceRollerWotR.StatArrayCalculation.Stat;

namespace DiceRollerWotR
{
    public class Main
    {
        /*
        [HarmonyPatch(typeof(LocalMapPCView))]
        [HarmonyPatch("OnPointerClick")]
        static class LocalMapPCView_OnPointerClick_Patch
        {

            private static bool Prefix(LocalMapPCView __instance, PointerEventData eventData)
        {

        }
    }
*/

        public static bool enabled;

        public static StatArrayType arrayType = StatArrayType.ThreeDice;

        public static System.Random randomGenerator;

        public static UnityModManager.ModEntry.ModLogger logger;

        static Settings settings;

        static Harmony harmonyInstance;


        [System.Diagnostics.Conditional("DEBUG")]
        static void EnableGameLogging()
        {
            if (Owlcat.Runtime.Core.Logging.Logger.Instance.Enabled) return;

            // Code taken from GameStarter.Awake(). PF:K logging can be enabled with command line flags,
            // but when developing the mod it's easier to force it on.
            var dataPath = ApplicationPaths.persistentDataPath;
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Owlcat.Runtime.Core.Logging.Logger.Instance.Enabled = true;
            var text = Path.Combine(dataPath, "GameLog.txt");
            if (File.Exists(text))
            {
                File.Copy(text, Path.Combine(dataPath, "GameLogPrev.txt"), overwrite: true);
                File.Delete(text);
            }
            Owlcat.Runtime.Core.Logging.Logger.Instance.AddLogger(new UberLoggerFile("GameLogFull.txt", dataPath));
            Owlcat.Runtime.Core.Logging.Logger.Instance.AddLogger(new UberLoggerFilter(new UberLoggerFile("GameLog.txt", dataPath), LogSeverity.Warning, "MatchLight"));

            Owlcat.Runtime.Core.Logging.Logger.Instance.Enabled = true;
        }

        internal static void NotifyPlayer(string message, bool warning = false)
        {
            if (warning)
            {
                EventBus.RaiseEvent<IWarningNotificationUIHandler>((IWarningNotificationUIHandler h) => h.HandleWarning(message, true));
            }
            else
            {
                // Game.Instance.UI.DBattleLogManager.LogView.AddLogEntry(message, GameLogStrings.Instance.DefaultColor);
            }
        }



        // mod entry point, invoked from UMM
        static bool Load(UnityModManager.ModEntry modEntry)
        {
#if DEBUG
            try
            {
#endif
                logger = modEntry.Logger;
                randomGenerator = new System.Random(); // To-Do: reuse Seed? Is there a reason?
                modEntry.OnToggle = OnToggle;
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = OnSaveGUI;
                settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
                harmonyInstance = new Harmony(modEntry.Info.Id);
                harmonyInstance.PatchAll();
                StartMod();
#if DEBUG
            }
            catch (Exception e)
            {
                Log.Write(e.ToString());
            }
#endif

                return true;
            }

        static void StartMod()
        {
            RolledArray.stats = new StatArrayCalculation.StatArray(StatArrayCalculation.Stat.StatArrayType.ThreeDice);
            Log.Write($"{RolledArray.stats != null}");
            // SafeLoad(StateManager.Load, "State Manager");
        }

        public static bool isActive()
        {
            if (enabled && RolledArray.stats != null)
            
                return true;
            else
                return false;
        } 
         
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            /*
            if(!enabled && DiceRollerWotR.Patch.NewChar.levelUpStateData != null)
            {
                .ForEach()
            }
            */
            return true;
        } 


        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            var fixedWidth = new GUILayoutOption[1] { GUILayout.ExpandWidth(false) };
            // prevent Changing Values when in Character Stat Screen. 
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b><color=cyan>Current Rolled Stats: </color></b>", fixedWidth);
            foreach(StatType statType in  StatTypeHelper.Attributes)
            {
                GUILayout.Label("<b>"+statType.ToString()+"</b>: "+RolledArray.stats[statType], fixedWidth);

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            arrayType = (StatArrayType) GUILayout.SelectionGrid((int)arrayType,Enum.GetNames(typeof(StatArrayCalculation.Stat.StatArrayType)),1, fixedWidth);
            if (GUILayout.Button("Reroll", fixedWidth))
            {
                RolledArray.Reroll(arrayType);
            }
            GUILayout.EndHorizontal();
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
            //To-Do: Update Character Stat Change Screen on GUISave
        }


    } 


    public class Settings : UnityModManager.ModSettings
    {
        public bool SaveAfterEveryChange = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }
    }
}
