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
    // Token: 0x02000002 RID: 2
    public class Main : MBSubModuleBase
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        protected override void OnSubModuleLoad()
        {
            new Harmony("AnnoDomini1084").PatchAll();
            List<InitialStateOption> _initialStateOptions = (List<InitialStateOption>)TaleWorlds.MountAndBlade.Module.CurrentModule.GetType().GetField("_initialStateOptions", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(TaleWorlds.MountAndBlade.Module.CurrentModule);
            InitialStateOption story = _initialStateOptions.First((InitialStateOption x) => x.Id == "StoryModeNewGame");
            InitialStateOption sb = _initialStateOptions.First((InitialStateOption x) => x.Id == "SandBoxNewGame");
            bool flag = story != null;
            if (flag)
            {
                _initialStateOptions.Remove(story);
            }
            bool flag2 = sb != null;
            if (flag2)
            {
                _initialStateOptions.Remove(sb);
            }
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("SI", new TextObject("{=!}Start New Campaign", null), 3, delegate ()
            {
                MBGameManager.StartNewGame(new GameManager());
            }, () => new ValueTuple<bool, TextObject>(TaleWorlds.MountAndBlade.Module.CurrentModule.IsOnlyCoreContentEnabled, new TextObject("{=V8BXjyYq}Disabled during installation.", null)), null));
        }
    }
}
