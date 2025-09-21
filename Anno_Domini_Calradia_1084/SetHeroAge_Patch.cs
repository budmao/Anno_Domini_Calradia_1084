using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(Hero), "Age", MethodType.Getter)]
    public class SetHeroAgePatch
    {
        public static void Postfix(ref float __result)
        {
            // Adjust the hero's age by adding 18 years to reflect the 1066 start year
            __result += 18f;
        }
    }
}
