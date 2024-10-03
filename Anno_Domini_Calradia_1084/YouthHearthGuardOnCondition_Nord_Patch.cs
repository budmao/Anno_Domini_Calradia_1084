using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace Anno_Domini_Calradia_1084.Patches
{

    internal class YouthHearthGuardOnCondition_Nord_Patch
    {
        [HarmonyPatch(typeof(SandboxCharacterCreationContent))]
        [HarmonyPatch("YouthHearthGuardOnCondition")]
        public class SandboxCharacterCreationContentYouthHearthGuardOnConditionPatch
        {
            public static void Postfix(ref bool __result, SandboxCharacterCreationContent __instance)
            {
                if (__instance.GetSelectedCulture().StringId == "nord") __result = true;
            }
        }

    }
}
