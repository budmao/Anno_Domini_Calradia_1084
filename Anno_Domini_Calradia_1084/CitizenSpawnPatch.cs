using HarmonyLib;
using SandBox.CampaignBehaviors;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem.Settlements;

namespace Anno_Domini_Calradia_1084
{
    public class CitizenSpawnPatch
    {
        [HarmonyPatch(typeof(CommonTownsfolkCampaignBehavior), "LocationCharactersAreReadyToSpawn")]
        public static class LocationCharactersAreReadyToSpawnPatch
        {
            public static bool AlwaysReturnsFalse(Settlement settlement)
            {
                return false;
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                MethodInfo isCastleProp = AccessTools.PropertyGetter(typeof(Settlement), "IsCastle");
                MethodInfo alwaysFalseMethod = AccessTools.Method(typeof(LocationCharactersAreReadyToSpawnPatch), nameof(AlwaysReturnsFalse));

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(isCastleProp)) { yield return new CodeInstruction(OpCodes.Call, alwaysFalseMethod); }
                    else { yield return instruction; }
                }
            }
        }

        [HarmonyPatch(typeof(CommonTownsfolkCampaignBehavior), "GetSpawnRate")]
        public static class GetSpawnRatePatch
        {
            static void Postfix(Settlement settlement, ref float __result)
            {
                if (settlement.IsCastle)
                {
                    __result *= 2.0f;
                    if (__result < 0.15f && __result > 0.001f)
                        __result = 0.15f;
                }
            }
        }
    }
}