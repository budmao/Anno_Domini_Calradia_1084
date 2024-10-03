using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace Anno_Domini_Calradia_1084.CC
{
    [HarmonyPatch(typeof(SandboxCharacterCreationContent))] // Targeting the correct class
    public static class YouthGarrisonOnCondition_Patch
    {
        // Patch for YouthGarrisonOnCondition
        [HarmonyPatch("YouthGarrisonOnCondition")]
        [HarmonyPrefix]
        public static bool YouthGarrisonOnConditionPrefix(ref bool __result, SandboxCharacterCreationContent __instance)
        {
            string selectedCultureId = __instance.GetSelectedCulture().StringId;

            // Check for the specified cultures
            if (selectedCultureId == "empire" ||
                selectedCultureId == "vlandia" ||
                selectedCultureId == "svadia" ||
                selectedCultureId == "nord" ||
                selectedCultureId == "battania" ||
                selectedCultureId == "sturgia" ||
                selectedCultureId == "aserai" ||
                selectedCultureId == "khuzait")
            {
                __result = true; // Set result to true
                return false; // Skip the original method
            }

            // Set to false if none match, ensuring original method logic is preserved
            __result = false;
            return false; // Skip the original method
        }

        // Patch for YouthOtherGarrisonOnCondition
        [HarmonyPatch("YouthOtherGarrisonOnCondition")]
        [HarmonyPrefix]
        public static bool YouthOtherGarrisonOnConditionPrefix(ref bool __result, SandboxCharacterCreationContent __instance)
        {
            string selectedCultureId = __instance.GetSelectedCulture().StringId;

            // Check for the cultures that should return true
            if (selectedCultureId != "empire" &&
                selectedCultureId != "vlandia" &&
                selectedCultureId != "svadia" &&
                selectedCultureId != "nord" &&
                selectedCultureId != "battania" &&
                selectedCultureId != "sturgia" &&
                selectedCultureId != "aserai" &&
                selectedCultureId != "khuzait")
            {
                __result = true; // Set result to true
                return false; // Skip the original method
            }

            // Set to false if any match, ensuring original method logic is preserved
            __result = false;
            return false; // Skip the original method
        }
    }
}
