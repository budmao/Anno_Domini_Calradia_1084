using System.Linq;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(FactionHelper), "GenerateClanNameforPlayer")]
    internal class FactionHelper_Patch
    {
        public static void Postfix(ref TextObject __result)
        {
            // Get the culture of the player character
            CultureObject culture = CharacterObject.PlayerCharacter.Culture;

            // Check if the culture is Vlandian
            if (culture.StringId == "vlandia")
            {
                // Specify a specific settlement for Vlandians (e.g., "Chareux")
                Settlement specificSettlement = Settlement.All.FirstOrDefault(s => s.Name.ToString() == "Chareux");

                // If the settlement doesn't exist, you can fallback to a default one or null
                if (specificSettlement == null)
                {
                    specificSettlement = null; // Or set to a fallback settlement if needed
                }

                // Generate a clan name using the Vlandian culture and the specified settlement
                __result = NameGenerator.Current.GenerateClanName(culture, specificSettlement);
            }
        }
    }
}
