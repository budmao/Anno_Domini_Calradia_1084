using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using HarmonyLib;
using Anno_Domini_Calradia_1084.CC;
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

                var currentModule = TaleWorlds.MountAndBlade.Module.CurrentModule;
                List<InitialStateOption> initialStateOptions = (List<InitialStateOption>)currentModule.GetType()
                    .GetField("_initialStateOptions", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(currentModule);

                InitialStateOption story = initialStateOptions.FirstOrDefault(x => x.Id == "StoryModeNewGame");
                InitialStateOption sb = initialStateOptions.FirstOrDefault(x => x.Id == "SandBoxNewGame");

                if (story != null)
                {
                    initialStateOptions.Remove(story);
                }

                if (sb != null)
                {
                    initialStateOptions.Remove(sb);
                }

                currentModule.AddInitialStateOption(new InitialStateOption("ADC", new TextObject("{=!}Start New Campaign", null), 3, delegate ()
                {
                    MBGameManager.StartNewGame(new GameManager_AD());
                }, () => new ValueTuple<bool, TextObject>(currentModule.IsOnlyCoreContentEnabled, new TextObject("{=V8BXjyYq}Disabled during installation.", null)), null));

                Log("Successfully loaded Anno Domini 1084 main module and replaced initial state options.");
            }
            catch (Exception ex)
            {
                Log($"Error loading Anno Domini 1084 main module: {ex}");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter campaignStarter)
            {
                try
                {
                    // Register the custom wage model
                    campaignStarter.AddModel(new WageModel());
                    Log("Successfully registered Anno Domini WageModel.");

                    // Register the custom upgrade model
                    campaignStarter.AddModel(new UpgradeModel());
                    Log("Successfully registered Anno Domini UpgradeModel.");
                }
                catch (Exception ex)
                {
                    Log($"Error registering campaign models: {ex}");
                }
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