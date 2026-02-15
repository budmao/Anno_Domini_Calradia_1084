using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(FightTournamentGame), "GetTournamentPrize")]
    public class CachePossibleEliteRewardItems_Patch
    {
        static void Prefix(FightTournamentGame __instance)
        {
            FieldInfo cacheField = typeof(FightTournamentGame).GetField("_possibleEliteRewardItemObjectsCache", BindingFlags.NonPublic | BindingFlags.Instance);
            if (cacheField == null) return;

            var rewardCache = (MBList<ItemObject>)cacheField.GetValue(__instance);

            if (rewardCache != null && rewardCache.Count > 0) return;

            rewardCache = new MBList<ItemObject>();

            foreach (string objectName in new string[]
            {
                "noble_horse_southern",
                "noble_horse_imperial",
                "noble_horse_western",
                "noble_horse_eastern",
                "noble_horse_battania",
                "noble_horse_northern",
                "special_camel"
            })
            {
                ItemObject item = Game.Current.ObjectManager.GetObject<ItemObject>(objectName);
                if (item != null)
                {
                    rewardCache.Add(item);
                }
            }

            rewardCache.Sort((x, y) => x.Value.CompareTo(y.Value));
            cacheField.SetValue(__instance, rewardCache);
        }
    }
}