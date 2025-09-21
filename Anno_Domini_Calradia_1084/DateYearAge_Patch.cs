using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(Hero), "Age", MethodType.Getter)]
    public class DateYearPatch2
    {
        public static void Postfix(ref float __result)
        {
            // If the hero's age is greater than 18, subtract 18 to align with the new start date
            if (__result > 18f)
            {
                __result -= 18f;
            }
        }
    }
}