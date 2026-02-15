using System;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(FactionHelper), "GenerateClanNameforPlayer")]
    internal class FactionHelper_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref TextObject __result)
        {
            CultureObject culture = CharacterObject.PlayerCharacter.Culture;

            try
            {
                __result = NameGenerator.Current.GenerateClanName(culture, null);
            }
            catch (Exception)
            {
                // Fallback if name generation fails (e.g. missing culture name data)
                __result = NameGenerator.Current.GenerateClanName(
                    CharacterObject.PlayerCharacter.Culture,
                    Hero.MainHero?.HomeSettlement);
            }

            return false;
        }
    }
}