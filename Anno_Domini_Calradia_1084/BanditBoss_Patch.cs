using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.Party.PartyComponents;

namespace Anno_Domini_Calradia_1084
{
    // Helper to track boss status between prefix and postfix
    public static class BanditBossHelper
    {
        public static bool IsBossParty { get; set; }
    }

    // Prefix: 10% chance to swap bandit template to boss variant
    [HarmonyPatch(typeof(BanditPartyComponent), "CreateBanditParty")]
    public class BanditBoss_Patch
    {
        static void Prefix(ref PartyTemplateObject pt, bool isBossParty)
        {
            BanditBossHelper.IsBossParty = false;

            // Don't modify if the game already marked this as a boss party
            if (isBossParty)
                return;

            if (pt == null)
                return;

            string templateId = pt.StringId;

            if (!IsBanditFactionTemplate(templateId))
                return;

            //Main.DebugLog($"[BanditBoss] Bandit party spawning with template '{templateId}'");

            // 10% chance to upgrade to boss template
            if (MBRandom.RandomFloat < 0.1f)
            {
                PartyTemplateObject bossTemplate = GetBossTemplate(templateId);
                if (bossTemplate != null)
                {
                    pt = bossTemplate;
                    BanditBossHelper.IsBossParty = true;
                    //Main.DebugLog($"[BanditBoss] BOSS SPAWN! '{templateId}' -> '{bossTemplate.StringId}'");
                }
                else
                {
                    Main.DebugLog($"[BanditBoss] Boss roll succeeded but template '{templateId}_boss' not found!");
                }
            }
        }

        private static bool IsBanditFactionTemplate(string templateId)
        {
            return templateId != null &&
                   !templateId.Contains("_boss") &&
                   (templateId.Contains("forest_bandits") ||
                    templateId.Contains("mountain_bandits") ||
                    templateId.Contains("desert_bandits") ||
                    templateId.Contains("steppe_bandits") ||
                    templateId.Contains("sea_raiders"));
        }

        private static PartyTemplateObject GetBossTemplate(string originalTemplateId)
        {
            string bossTemplateId = originalTemplateId + "_boss";
            return MBObjectManager.Instance.GetObject<PartyTemplateObject>(bossTemplateId);
        }
    }

    // Postfix: rename boss bandit parties
    [HarmonyPatch(typeof(BanditPartyComponent), "CreateBanditParty")]
    public class BanditBoss_NamePatch
    {
        static void Postfix(MobileParty __result, PartyTemplateObject pt)
        {
            if (!BanditBossHelper.IsBossParty || __result == null || pt == null)
                return;

            string customName = GetBossBanditName(pt.StringId);

            if (!string.IsNullOrEmpty(customName))
            {
                __result.Party.SetCustomName(new TextObject(customName));
                //Main.DebugLog($"[BanditBoss] Renamed boss party to '{customName}' (template: {pt.StringId})");
            }

            BanditBossHelper.IsBossParty = false;
        }

        private static string GetBossBanditName(string templateId)
        {
            if (templateId.Contains("forest_bandits"))
                return "Knyaz Lesov";
            if (templateId.Contains("mountain_bandits"))
                return "Buruzagi";
            if (templateId.Contains("desert_bandits"))
                return "Ra'is al-Ghuzat";
            if (templateId.Contains("steppe_bandits"))
                return "Bagadar";
            if (templateId.Contains("sea_raiders"))
                return "Skipari";

            return null;
        }
    }
}