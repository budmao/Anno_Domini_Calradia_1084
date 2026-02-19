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
    // Prefix: swap the party template based on settlement culture
    [HarmonyPatch(typeof(BanditPartyComponent), "CreateLooterParty")]
    public class RegionalLooters_Patch
    {
        static void Prefix(ref PartyTemplateObject pt, Settlement relatedSettlement)
        {
            if (relatedSettlement == null || relatedSettlement.Culture == null || pt == null)
                return;

            // Only replace vanilla looter parties — skip pirates, naval bandits, etc.
            if (pt.StringId != "looters_template")
            {
                //Main.DebugLog($"[Looters] Skipping non-looter template '{pt.StringId}' near {relatedSettlement.Name}");
                return;
            }

            string cultureId = relatedSettlement.Culture.StringId;
            string originalTemplate = pt.StringId;

            // Try to get regional template
            PartyTemplateObject cultureTemplate = GetLooterTemplateForCulture(cultureId);
            if (cultureTemplate != null)
            {
                pt = cultureTemplate;

                // 10% chance to upgrade to elite template
                if (MBRandom.RandomFloat < 0.1f)
                {
                    PartyTemplateObject eliteTemplate = GetEliteLooterTemplateForCulture(cultureId);
                    if (eliteTemplate != null)
                    {
                        pt = eliteTemplate;
                        //Main.DebugLog($"[Looters] ELITE SPAWN near {relatedSettlement.Name}: '{originalTemplate}' -> '{pt.StringId}'");
                        return;
                    }
                }

                //Main.DebugLog($"[Looters] Regional spawn near {relatedSettlement.Name}: '{originalTemplate}' -> '{pt.StringId}'");
            }
            else
            {
                Main.DebugLog($"[Looters] No regional template for culture '{cultureId}', keeping '{originalTemplate}'");
            }
        }

        private static PartyTemplateObject GetLooterTemplateForCulture(string cultureId)
        {
            string templateId = "looters_template_" + cultureId;
            PartyTemplateObject template = MBObjectManager.Instance.GetObject<PartyTemplateObject>(templateId);

            if (template == null)
            {
                template = MBObjectManager.Instance.GetObject<PartyTemplateObject>("looters_template");
            }

            return template;
        }

        private static PartyTemplateObject GetEliteLooterTemplateForCulture(string cultureId)
        {
            string templateId = "looters_template_" + cultureId + "_elite";
            return MBObjectManager.Instance.GetObject<PartyTemplateObject>(templateId);
        }
    }

    // Postfix: rename the party based on culture
    [HarmonyPatch(typeof(BanditPartyComponent), "CreateLooterParty")]
    public class RegionalLooters_NamePatch
    {
        static void Postfix(MobileParty __result, PartyTemplateObject pt)
        {
            if (__result == null || pt == null)
                return;

            string templateId = pt.StringId;

            if (templateId.StartsWith("looters_template_"))
            {
                string cultureId = templateId.Replace("looters_template_", "").Replace("_elite", "");
                string customName = GetCustomLooterName(cultureId);

                if (!string.IsNullOrEmpty(customName))
                {
                    __result.Party.SetCustomName(new TextObject(customName));
                    //Main.DebugLog($"[Looters] Renamed party to '{customName}' (template: {templateId})");
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
}