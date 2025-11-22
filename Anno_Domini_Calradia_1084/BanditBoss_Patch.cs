using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.Localization;

namespace Anno_Domini_Calradia_1084
{
    // Helper to track if party should get boss name
    public static class BanditBossHelper
    {
        public static bool IsBossParty { get; set; }
    }

    // Patch to add 5% chance for elite templates with boss units for proper bandit factions
    [HarmonyPatch(typeof(MobileParty), "InitializeMobilePartyAroundPosition", new Type[] { typeof(PartyTemplateObject), typeof(TaleWorlds.Library.Vec2), typeof(float), typeof(float), typeof(int) })]
    public class BanditBoss_Patch
    {
        static void Prefix(ref PartyTemplateObject pt, MobileParty __instance)
        {
            // Reset flag
            BanditBossHelper.IsBossParty = false;

            // Only modify bandit parties (not looters, they have their own system)
            if (__instance != null && __instance.IsBandit && pt != null)
            {
                string templateId = pt.StringId;

                // Check if this is a proper bandit faction template
                if (IsBanditFactionTemplate(templateId))
                {
                    // 10% chance to upgrade to elite template with boss unit
                    if (MBRandom.RandomFloat < 0.1f)
                    {
                        PartyTemplateObject eliteTemplate = GetEliteBanditTemplate(templateId);
                        if (eliteTemplate != null)
                        {
                            pt = eliteTemplate;
                            BanditBossHelper.IsBossParty = true;
                        }
                    }
                }
            }
        }

        private static bool IsBanditFactionTemplate(string templateId)
        {
            // Check if this is a known bandit faction template
            return templateId != null && (
                   templateId.Contains("forest_bandits") ||
                   templateId.Contains("mountain_bandits") ||
                   templateId.Contains("desert_bandits") ||
                   templateId.Contains("steppe_bandits") ||
                   templateId.Contains("sea_raiders"));
        }

        private static PartyTemplateObject GetEliteBanditTemplate(string originalTemplateId)
        {
            // Build elite template ID by appending "_boss"
            string eliteTemplateId = originalTemplateId + "_boss";
            PartyTemplateObject template = MBObjectManager.Instance.GetObject<PartyTemplateObject>(eliteTemplateId);

            return template; // Returns null if not found, which is fine
        }
    }

    // Patch to rename boss bandit parties
    [HarmonyPatch(typeof(MobileParty), "InitializeMobilePartyAroundPosition", new Type[] { typeof(PartyTemplateObject), typeof(TaleWorlds.Library.Vec2), typeof(float), typeof(float), typeof(int) })]
    public class BanditBoss_NamePatch
    {
        static void Postfix(MobileParty __instance, PartyTemplateObject pt)
        {
            // Only rename if this was marked as a boss party
            if (BanditBossHelper.IsBossParty && __instance != null && __instance.IsBandit && pt != null)
            {
                string templateId = pt.StringId;

                if (templateId != null && templateId.Contains("_boss"))
                {
                    string customName = GetBossBanditName(templateId);

                    if (!string.IsNullOrEmpty(customName))
                    {
                        __instance.SetCustomName(new TextObject(customName));
                    }
                }

                // Reset flag
                BanditBossHelper.IsBossParty = false;
            }
        }

        private static string GetBossBanditName(string templateId)
        {
            if (templateId.Contains("forest_bandits"))
            {
                return "Knyaz Lesov";
            }
            else if (templateId.Contains("mountain_bandits"))
            {
                return "Buruzagi";
            }
            else if (templateId.Contains("desert_bandits"))
            {
                return "Ra'is al-Ghuzat";
            }
            else if (templateId.Contains("steppe_bandits"))
            {
                return "Bagadar";
            }
            else if (templateId.Contains("sea_raiders"))
            {
                return "Skipari";
            }

            return null;
        }
    }
}