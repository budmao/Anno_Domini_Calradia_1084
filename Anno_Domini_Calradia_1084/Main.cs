using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Anno_Domini_Calradia_1084.CC;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Anno_Domini_Calradia_1084
{
    public class Main : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
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
        }
    }
}
