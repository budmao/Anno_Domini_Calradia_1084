using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Anno_Domini_Calradia_1084
{
    // Patch to capture the settlement when it's selected for looter spawning
    [HarmonyPatch(typeof(BanditSpawnCampaignBehavior), "SelectARandomSettlementForLooterParty")]
    public class RegionalLooters_Patch
    {
        static void Postfix(ref Settlement __result)
        {
            // Store the selected settlement so we can use it when initializing the party
            if (__result != null)
            {
                RegionalLootersHelper.LastSelectedSettlement = __result;
            }
        }
    }

    // Patch to replace the party template based on settlement culture
    [HarmonyPatch(typeof(MobileParty), "InitializeMobilePartyAroundPosition", new Type[] { typeof(PartyTemplateObject), typeof(CampaignVec2), typeof(float), typeof(float) })]
    public class RegionalLooters_InitPatch
    {
        static void Prefix(ref PartyTemplateObject pt, MobileParty __instance)
        {
            // Only modify looter parties that have a stored settlement
            if (__instance != null &&
                __instance.IsBandit &&
                RegionalLootersHelper.LastSelectedSettlement != null)
            {
                Settlement settlement = RegionalLootersHelper.LastSelectedSettlement;

                // Get culture-specific template if settlement has a culture
                if (settlement.Culture != null)
                {
                    PartyTemplateObject cultureTemplate = GetLooterTemplateForCulture(settlement.Culture);
                    if (cultureTemplate != null)
                    {
                        pt = cultureTemplate;

                        // 10% chance to upgrade to elite template with boss unit
                        if (MBRandom.RandomFloat < 0.1f)
                        {
                            PartyTemplateObject eliteTemplate = GetEliteLooterTemplateForCulture(settlement.Culture);
                            if (eliteTemplate != null)
                            {
                                pt = eliteTemplate;
                            }
                        }
                    }
                }

                // Clear the stored settlement after use
                RegionalLootersHelper.LastSelectedSettlement = null;
            }
        }

        private static PartyTemplateObject GetLooterTemplateForCulture(CultureObject culture)
        {
            // Build template ID based on culture: "looters_template_empire", "looters_template_vlandia", etc.
            string templateId = "looters_template_" + culture.StringId;
            PartyTemplateObject template = MBObjectManager.Instance.GetObject<PartyTemplateObject>(templateId);

            // Fallback to default looter template if culture-specific one doesn't exist
            if (template == null)
            {
                template = MBObjectManager.Instance.GetObject<PartyTemplateObject>("looters_template");
            }

            return template;
        }

        private static PartyTemplateObject GetEliteLooterTemplateForCulture(CultureObject culture)
        {
            // Build elite template ID: "looters_template_empire_elite", etc.
            string templateId = "looters_template_" + culture.StringId + "_elite";
            PartyTemplateObject template = MBObjectManager.Instance.GetObject<PartyTemplateObject>(templateId);

            return template; // Returns null if not found, which is fine
        }
    }

    // Patch to rename looter parties based on their culture
    [HarmonyPatch(typeof(MobileParty), "InitializeMobilePartyAroundPosition", new Type[] { typeof(PartyTemplateObject), typeof(CampaignVec2), typeof(float), typeof(float) })]
    public class RegionalLooters_NamePatch
    {
        static void Postfix(MobileParty __instance, PartyTemplateObject pt)
        {
            // Only rename looter parties with culture-specific templates
            if (__instance != null && __instance.IsBandit && pt != null)
            {
                string templateId = pt.StringId;

                // Check if this is a culture-specific looter template
                if (templateId.StartsWith("looters_template_"))
                {
                    string cultureId = templateId.Replace("looters_template_", "").Replace("_elite", "");
                    string customName = GetCustomLooterName(cultureId);

                    if (!string.IsNullOrEmpty(customName))
                    {
                        __instance.Party.SetCustomName(new TaleWorlds.Localization.TextObject(customName));
                    }
                }
            }
        }

        private static string GetCustomLooterName(string cultureId)
        {
            switch (cultureId)
            {
                case "empire": return "Lestes";
                case "vlandia": return "Grassatores";
                case "sturgia": return "Tati";
                case "aserai": return "Lussus";
                case "khuzait": return "Yagmaci";
                case "battania": return "Ropairi";
                case "svadia": return "Grassatores";
                case "balion": return "Grassatores";
                case "nord": return "Ransmenn";
                default: return null;
            }
        }
    }

    // Helper class to pass data between patches
    public static class RegionalLootersHelper
    {
        public static Settlement LastSelectedSettlement { get; set; }
    }
}