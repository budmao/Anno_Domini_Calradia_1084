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

        protected override void OnBeforeInitialModuleScreenSetAsRootScreen()
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
                    Log("Removed StoryModeNewGame option.");
                }

                // Replace sandbox option with renamed version
                InitialStateOption sb = initialStateOptions.FirstOrDefault(x => x.Id == "SandBoxNewGame");
                if (sb != null)
                {
                    // Extract the action and condition from the existing sandbox option via reflection
                    var actionField = typeof(InitialStateOption).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .FirstOrDefault(f => f.FieldType == typeof(Action));
                    var conditionField = typeof(InitialStateOption).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .FirstOrDefault(f => f.FieldType == typeof(Func<ValueTuple<bool, TextObject>>));

                    Action sbAction = actionField != null ? (Action)actionField.GetValue(sb) : null;
                    Func<ValueTuple<bool, TextObject>> sbCondition = conditionField != null
                        ? (Func<ValueTuple<bool, TextObject>>)conditionField.GetValue(sb)
                        : null;

                    // Remove the original
                    initialStateOptions.Remove(sb);

                    if (sbAction != null)
                    {
                        // Add replacement with same action but new name
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
                        Log("Replaced SandBoxNewGame with Start New Campaign.");
                    }
                    else
                    {
                        Log("WARNING: Could not extract action from SandBoxNewGame option.");
                    }
                }
                else
                {
                    Log("WARNING: SandBoxNewGame option not found.");
                }
            }
            catch (Exception ex)
            {
                Log($"Error replacing initial state options: {ex}");
            }
        }

        private static void Log(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(LogPath, $"[{timestamp}] {message}{Environment.NewLine}");
            }
            catch
            {
                // Prevent logging errors from interrupting gameplay
            }
        }
    }
}