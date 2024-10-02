using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent; // Ensure this namespace is correct
using TaleWorlds.CampaignSystem; // Add this if needed for other dependencies

namespace Anno_Domini_Calradia_1084
{
    internal class YouthGroomOnCondition_Svadia_Vlandia_Patch
    {
        [HarmonyPatch(typeof(SandboxCharacterCreationContent))]
        [HarmonyPatch("YouthGroomOnCondition")]
        public class SandboxCharacterCreationContentYouthGroomOnConditionPatch
        {
            public static void Postfix(ref bool __result, SandboxCharacterCreationContent __instance)
            {
                // Access the _familyOccupationType field
                var occupationTypeField = AccessTools.Field(typeof(SandboxCharacterCreationContent), "_familyOccupationType");
                var occupationType = occupationTypeField.GetValue(__instance);

                // Check for Svadia or Vlandia culture and Retainer occupation
                if ((__instance.GetSelectedCulture().StringId == "svadia" ||
                     __instance.GetSelectedCulture().StringId == "vlandia") &&
                    occupationType != null &&
                    occupationType.ToString() == "Retainer") // Ensure correct comparison
                {
                    __result = true;
                }
                else
                {
                    __result = false; // Explicitly set to false if conditions are not met
                }
            }
        }
    }
}
