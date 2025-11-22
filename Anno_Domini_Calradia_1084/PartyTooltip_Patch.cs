using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Localization;
using Anno_Domini_Calradia_1084_WageModel;

namespace Anno_Domini_Calradia_1084.Patches
{
    /// <summary>
    /// Adds herd breakdown to party tooltips
    /// Gets data from Anno Domini Food Consumption Mod
    /// Inserts before "Hold Alt" message with spacing
    /// RESPECTS SCOUTING/INSPECTION LIKE TROOPS
    /// </summary>
    [HarmonyPatch(typeof(TooltipRefresherCollection), "RefreshMobilePartyTooltip")]
    public class PartyTooltip_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
        {
            try
            {
                // Safety checks
                if (propertyBasedTooltipVM == null) return;
                if (args == null || args.Length < 3) return; // ⭐ Need at least 3 args
                if (!(args[0] is MobileParty mobileParty)) return;
                if (mobileParty.Party == null) return;

                // ⭐ GET VISIBILITY FLAGS (same as troops use)
                bool flag = (bool)args[1];   // Is player party or allied
                bool flag2 = (bool)args[2];  // Some condition
                bool isInspected = mobileParty.IsInspected;

                // ⭐ CRITICAL: Use same logic as troops
                bool showDetails = flag || isInspected || !flag2;

                // Get herd counts
                var herdCounts = Anno_Domini_Calradia_1084_FoodConsumptionModel.GetHerdCounts(mobileParty);

                // Only add if party has animals
                if (herdCounts.TotalHerd == 0) return;

                // Get the tooltip properties list
                var list = propertyBasedTooltipVM.TooltipPropertyList;
                if (list == null || list.Count == 0) return;

                // Insert before the last 2 items (separator + info messages)
                int insertIndex = Math.Max(0, list.Count - 2);

                // Create the properties to insert
                var newProperties = new List<TooltipProperty>();

                // Add separator (above herd section)
                newProperties.Add(new TooltipProperty(
                    string.Empty,
                    string.Empty,
                    -1,
                    false,
                    TooltipProperty.TooltipPropertyFlags.None
                ));

                // ⭐ BRANCH: Show details OR question marks
                if (showDetails)
                {
                    // === SHOW FULL DETAILS (inspected/scouted) ===

                    // Add herd header with count in right column (bold yellow)
                    TextObject herdTitle = new TextObject("{=ADC_Herd}Herd");
                    TextObject herdCount = new TextObject("{=ADC_HerdCount}({HERD_COUNT})");
                    herdCount.SetTextVariable("HERD_COUNT", herdCounts.TotalHerd);

                    newProperties.Add(new TooltipProperty(
                        herdTitle.ToString(),       // Left column: "Herd"
                        herdCount.ToString(),       // Right column: "(50)"
                        0,
                        false,
                        TooltipProperty.TooltipPropertyFlags.None
                    ));

                    // Add separator line
                    newProperties.Add(new TooltipProperty(
                        string.Empty,
                        string.Empty,
                        0,
                        false,
                        TooltipProperty.TooltipPropertyFlags.RundownSeperator
                    ));

                    // Add breakdowns
                    if (herdCounts.Mounts > 0)
                    {
                        newProperties.Add(new TooltipProperty(
                            new TextObject("{=ADC_Mounts}Mounts").ToString(),
                            herdCounts.Mounts.ToString(),
                            0,
                            false,
                            TooltipProperty.TooltipPropertyFlags.None
                        ));
                    }

                    if (herdCounts.PackAnimals > 0)
                    {
                        newProperties.Add(new TooltipProperty(
                            new TextObject("{=ADC_PackAnimals}Pack Animals").ToString(),
                            herdCounts.PackAnimals.ToString(),
                            0,
                            false,
                            TooltipProperty.TooltipPropertyFlags.None
                        ));
                    }

                    if (herdCounts.Livestock > 0)
                    {
                        newProperties.Add(new TooltipProperty(
                            new TextObject("{=ADC_Livestock}Livestock").ToString(),
                            herdCounts.Livestock.ToString(),
                            0,
                            false,
                            TooltipProperty.TooltipPropertyFlags.None
                        ));
                    }
                }
                else
                {
                    // === SHOW QUESTION MARKS (not inspected) ===

                    int totalSize = herdCounts.TotalHerd;
                    string questionMarks;

                    // Match troop question mark logic exactly
                    if (totalSize >= 1000)
                        questionMarks = "????";
                    else if (totalSize >= 100)
                        questionMarks = "???";
                    else if (totalSize >= 10)
                        questionMarks = "??";
                    else
                        questionMarks = "?";

                    TextObject herdTitle = new TextObject("{=ADC_Herd}Herd");

                    newProperties.Add(new TooltipProperty(
                        herdTitle.ToString(),       // Left column: "Herd"
                        $"({questionMarks})",       // Right column: "(??)"
                        0,
                        false,
                        TooltipProperty.TooltipPropertyFlags.None
                    ));
                }

                // Add separator (below herd section) for spacing
                newProperties.Add(new TooltipProperty(
                    string.Empty,
                    string.Empty,
                    -1,
                    false,
                    TooltipProperty.TooltipPropertyFlags.None
                ));

                // Insert all properties at the found position
                for (int i = 0; i < newProperties.Count; i++)
                {
                    list.Insert(insertIndex + i, newProperties[i]);
                }
            }
            catch (Exception ex)
            {
                // Fail silently if Anno Domini mod not loaded or any other error
            }
        }
    }
}