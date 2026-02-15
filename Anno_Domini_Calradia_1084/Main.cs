using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Anno_Domini_Calradia_1084
{
    public class Main : MBSubModuleBase
    {
        // ── Debug toggle ──────────────────────────────────────────────
        // Set to true to enable verbose logging of patch activity.
        // Set to false for release builds.
        public static bool DebugMode = true;
        // ──────────────────────────────────────────────────────────────

        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount and Blade II Bannerlord",
            "Configs",
            "Anno_Domini_Calradia_1084_Debug.log"
        );

        protected override void OnSubModuleLoad()
        {
            try
            {
                new Harmony("AnnoDomini1084").PatchAll();
                Log("Harmony patches applied successfully.");
            }
            catch (Exception ex)
            {
                Log($"Error loading Anno Domini 1084 main module: {ex}");
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            try
            {
                var currentModule = TaleWorlds.MountAndBlade.Module.CurrentModule;
                List<InitialStateOption> initialStateOptions = (List<InitialStateOption>)currentModule.GetType()
                    .GetField("_initialStateOptions", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(currentModule);

                // Remove story mode option
                InitialStateOption story = initialStateOptions.FirstOrDefault(x => x.Id == "StoryModeNewGame");
                if (story != null)
                {
                    initialStateOptions.Remove(story);
                }

                // Replace sandbox option with renamed version
                InitialStateOption sb = initialStateOptions.FirstOrDefault(x => x.Id == "SandBoxNewGame");
                if (sb != null)
                {
                    var actionField = typeof(InitialStateOption).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .FirstOrDefault(f => f.FieldType == typeof(Action));
                    var conditionField = typeof(InitialStateOption).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .FirstOrDefault(f => f.FieldType == typeof(Func<ValueTuple<bool, TextObject>>));

                    Action sbAction = actionField != null ? (Action)actionField.GetValue(sb) : null;
                    Func<ValueTuple<bool, TextObject>> sbCondition = conditionField != null
                        ? (Func<ValueTuple<bool, TextObject>>)conditionField.GetValue(sb)
                        : null;

                    initialStateOptions.Remove(sb);

                    if (sbAction != null)
                    {
                        currentModule.AddInitialStateOption(new InitialStateOption(
                            "ADC",
                            new TextObject("{=!}Start New Campaign", null),
                            3,
                            sbAction,
                            sbCondition ?? (() => new ValueTuple<bool, TextObject>(
                                currentModule.IsOnlyCoreContentEnabled,
                                new TextObject("{=V8BXjyYq}Disabled during installation.", null))),
                            null
                        ));
                    }
                }

                Log("Main menu options replaced successfully.");
            }
            catch (Exception ex)
            {
                Log($"Error replacing initial state options: {ex}");
            }
        }

        public static void Log(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(LogPath, $"[{timestamp}] {message}{Environment.NewLine}");
            }
            catch
            {
            }
        }

        public static void DebugLog(string message)
        {
            if (DebugMode)
            {
                Log("[DEBUG] " + message);
            }
        }
    }
}