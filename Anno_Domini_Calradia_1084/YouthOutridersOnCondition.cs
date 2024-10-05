using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(SandboxCharacterCreationContent))] // Targeting the correct class
    public static class YouthOutridersOnCondition_Patch
    {
        // Patch for YouthOutridersOnCondition
        [HarmonyPatch("YouthOutridersOnCondition")]
        [HarmonyPrefix]
        public static bool YouthOutridersOnConditionPrefix(ref bool __result, SandboxCharacterCreationContent __instance)
        {
            string selectedCultureId = __instance.GetSelectedCulture().StringId;

            // Check for the specified cultures
            if (selectedCultureId == "empire" ||
                selectedCultureId == "khuzait" ||
                selectedCultureId == "aserai")
            {
                __result = true; // Set result to true
                return false; // Skip the original method
            }

            // Set to false if none match, ensuring original method logic is preserved
            __result = false;
            return false; // Skip the original method
        }

        // Patch for YouthOtherOutridersOnCondition
        [HarmonyPatch("YouthOtherOutridersOnCondition")]
        [HarmonyPrefix]
        public static bool YouthOtherOutridersnOnConditionPrefix(ref bool __result, SandboxCharacterCreationContent __instance)
        {
            string selectedCultureId = __instance.GetSelectedCulture().StringId;

            // Check for the cultures that should return true
            if (selectedCultureId != "empire" &&
                selectedCultureId != "khuzait" &&
                selectedCultureId != "aserai")
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
