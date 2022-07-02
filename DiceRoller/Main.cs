// Copyright (c) 2019 v1ld.git@gmail.com
// Copyright (c) 2019 Jennifer Messerly
// This code is licensed under MIT license (see LICENSE for details)

using System; 
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
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic;

namespace DiceRollerWotR
{
#if DEBUG
    [EnableReloading]
#endif

    public class Main
    {

        public static bool enabled;

        public static System.Random randomGenerator;

        public static UnityModManager.ModEntry.ModLogger logger;


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
#if DEBUG
                modEntry.OnUnload = Unload;
#endif
                Accessor.Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
                if (Accessor.Settings == null)
                    Accessor.Settings = new Settings();

                harmonyInstance = new Harmony(modEntry.Info.Id);
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
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

#if DEBUG
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmonyInstance.UnpatchAll();

            return true;
        }
#endif


        static void StartMod()
        {
            RolledArray.Reroll(Accessor.Settings.DiceExpression);
#if DEBUG
            Log.Write($"{RolledArray.Stats != null}");
#endif
            // SafeLoad(StateManager.Load, "State Manager");

        }

        public static bool isActive()
        {
            if (enabled && RolledArray.Stats != null)

                return true;
            else
                return false;
        }

        public static bool CheckForAllowance(LevelUpState levelupState, LevelUpState.CharBuildMode mode)
        {
            return Main.isActive()
                && (levelupState.IsFirstCharacterLevel)
                && (mode == LevelUpState.CharBuildMode.SetName
                    
                    || Accessor.Settings.IsLoreCompanionAllowed(levelupState.IsLoreCompanion && mode == LevelUpState.CharBuildMode.Respec)
                    || Accessor.Settings.IsCustomCompanionAllowed(levelupState.Unit.Unit.IsCustomCompanion())
                    || Accessor.Settings.IsPetCompanionAllowed(levelupState.Unit.Unit.IsPet)
                    || Accessor.Settings.IsEnemyAllowed(levelupState.Unit.Unit.IsPlayersEnemy));


        }


        private static bool searchedRespecMain = false;
        private static Type respecMain;

        public static bool isRespecActive
        {
            get
            {
                if (!searchedRespecMain)
                {
#if DEBUG
                    Log.Write("TestOutput: "+typeof(Main).FullName+" : " +typeof(Main).Name);
#endif
                    respecMain = UnityModManager.FindMod("RespecWrath")?.Assembly.GetType("RespecModBarley.Main"); // Helpers.SearchAssemblyFor("RespecModBarley.Main", @"Download_This_RespecWrath\RespecWrath.dll");
                    searchedRespecMain = true;
#if DEBUG
                    Log.Write("Searched For RespecWrath.");
                    if (respecMain != null)
                        Log.Write("Found.");
                    else
                        Log.Write("Not Found.");
#endif
                }
                if (searchedRespecMain && respecMain != null) 
                {
#if DEBUG
                    Log.Write("Get current IsRespecValue.");
#endif
                    return Helpers.GetValueOfField<bool>(respecMain, "IsRespec");
                }
                return false;
            }
        }
    
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            if(enabled)
                Accessor.Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

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
            if (enabled)
            {
                try { 
                    Accessor.Settings.DrawGUI(modEntry);
                } catch(Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Accessor.Settings.Save(modEntry);
            //To-Do: Update Character Stat Change Screen on GUISave
        }


    } 



}
