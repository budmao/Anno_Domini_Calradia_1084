using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace Anno_Domini_Calradia_1084.Patches
{

    internal class YouthCavalryOnCondition_Svadia_Patch
    {
        [HarmonyPatch(typeof(SandboxCharacterCreationContent))]
        [HarmonyPatch("YouthCavalryOnCondition")]
        public class SandboxCharacterCreationContentYouthCavalryOnConditionPatch
        {
            public static void Postfix(ref bool __result, SandboxCharacterCreationContent __instance)
            {
                if (__instance.GetSelectedCulture().StringId == "svadia") __result = true;
            }
        }

    }
}
