using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

// make that females are selected last for leading parties

namespace Anno_Domini_Calradia_1084
{
    [HarmonyPatch(typeof(Hero))]
    [HarmonyPatch("CanLeadParty")]
    public class Hero_CanLeadPartyPatch
    {
        public static void Postfix(ref bool __result, Hero __instance)
        {
            if (__instance != null && __instance.IsLord && __instance.IsFemale)
            {
                __result = false;
            }
        }
    }


    // NPC females can lead parties in exceptional cases only
    [HarmonyPatch(typeof(HeroSpawnCampaignBehavior))]
    [HarmonyPatch("GetBestAvailableCommander")]
    public class HeroSpawnCampaignBehavior_GetBestAvailableCommander_Patch
    {
        public static void Postfix(ref Hero __result)
        {
            if (__result == null) return;
            Hero hero = __result;

            if (hero.IsFemale && hero.IsLord)
            {

                if (hero.Clan != null)
                {
                    // Clan Leader can lead
                    if (hero.Clan.Leader == hero) return;

                    // Kingdom Queen can lead - clan leader should cover this, but just in case
                    if (hero.Clan.Kingdom != null && hero.Clan.Kingdom.Leader == hero) return;
                }

                __result = null;
            }
        }
    }
}