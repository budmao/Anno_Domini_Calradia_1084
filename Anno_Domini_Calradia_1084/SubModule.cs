using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Anno_Domini_Calradia_1084
{
    public class SubModule : MBSubModuleBase
    {
        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            new Harmony("Anno_Domini_Calradia_1084").PatchAll();
            InformationManager.DisplayMessage(new InformationMessage("Anno Domini: Calradia 1084"));
        }

    }
}



