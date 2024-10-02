using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace ExampleMod
{
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
