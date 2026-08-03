using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(Clan), "UpdateBannerColorsAccordingToKingdom")]
    public static class BanditClanColorPatch
    {
        public static readonly Dictionary<string, (uint, uint)> Overrides = new Dictionary<string, (uint, uint)>
        {
            { "mountain_bandits", (0xFF830808, 0xFF2C4D86) },
            { "southern_pirates", (0xFF211F1F, 0xFFCCC4BF) },
        };

        static void Postfix(Clan __instance)
        {
            if (Overrides.TryGetValue(__instance.StringId, out var colors))
            {
                __instance.Color = colors.Item1;
                __instance.Color2 = colors.Item2;
            }
        }
    }
}