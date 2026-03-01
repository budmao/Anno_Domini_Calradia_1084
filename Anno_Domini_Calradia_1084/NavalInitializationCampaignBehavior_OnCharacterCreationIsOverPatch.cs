using HarmonyLib;
using NavalDLC.CampaignBehaviors;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(NavalInitializationCampaignBehavior), "OnCharacterCreationIsOver")]
    public class NavalInitializationCampaignBehavior_OnCharacterCreationIsOverPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}