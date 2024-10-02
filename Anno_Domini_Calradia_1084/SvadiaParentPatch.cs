using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace Anno_Domini_Calradia_1084
{

    internal class SvadiaParentPatch
    {
        [HarmonyPatch(typeof(SandboxCharacterCreationContent))]
        [HarmonyPatch("VlandianParentsOnCondition")]
        public class SandboxCharacterCreationContentVlandianParentsOnConditionPatch
        {
            public static void Postfix(ref bool __result, SandboxCharacterCreationContent __instance)
            {
                if (__instance.GetSelectedCulture().StringId == "svadia") __result = true;
            }
        }

    }
}
