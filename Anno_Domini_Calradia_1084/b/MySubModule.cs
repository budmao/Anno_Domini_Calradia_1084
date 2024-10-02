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

namespace ExampleMod
{
    public class MySubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            new Harmony("Anno_Domini_Calradia_1084").PatchAll();

            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Message",
                new TextObject("Message", null),
                9990,
                () => { InformationManager.DisplayMessage(new InformationMessage("Hello World!")); },
                () => { return (false, null); }));
        }
        internal class NordParentPatch
        {
            [HarmonyPatch(typeof(SandboxCharacterCreationContent))]
            [HarmonyPatch("SturgianParentsOnCondition")]
            public class SandboxCharacterCreationContentSturgianParentsOnConditionPatch
            {
                public static void Postfix(ref bool __result, SandboxCharacterCreationContent __instance)
                {
                    if (__instance.GetSelectedCulture().StringId == "nord") __result = true;
                }
            }

        }
    }
}



