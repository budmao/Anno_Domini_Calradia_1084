using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(CampaignTime), "GetYear", MethodType.Getter)]
    public class DateYearPatch
    {
        public static void Postfix(ref int __result, CampaignTime __instance)
        {
            // Subtract 18 years from the campaign year
            __result -= 18;
        }
    }
}