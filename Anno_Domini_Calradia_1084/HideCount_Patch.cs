using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Anno_Domini_Calradia_1084
{
    [HarmonyPatch(typeof(HorseComponent), "get_HideCount")]
    public class HideCount_Patch
    {
        static bool Prefix(HorseComponent __instance, ref int __result)
        {
            // New rule: if speed <= 11, hide count = 0
            if (__instance.Speed <= 11)
            {
                __result = 0;
                return false; // Skip original getter
            }

            return true; // Otherwise, run original getter
        }
    }
}
