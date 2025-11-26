using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace Anno_Domini_Calradia_1084
{
    public class UpgradeModel : DefaultPartyTroopUpgradeModel
    {
        // Gold cost multipliers for upgrades
        // Applied to the UPGRADE TARGET (what the troop is becoming)
        private const float IrregularUpgradeGoldMultiplier = 1.0f;   // Base cost
        private const float RegularUpgradeGoldMultiplier = 1.25f;    // 25% more expensive
        private const float RangedUpgradeGoldMultiplier = 1.25f;     // 25% more expensive
        private const float CrossbowUpgradeGoldMultiplier = 1.25f;   // 25% more expensive (same as ranged)
        private const float EliteUpgradeGoldMultiplier = 1.5f;       // 50% more expensive
        private const float NobleUpgradeGoldMultiplier = 1.5f;       // 50% more expensive

        // Experience cost multipliers for upgrades
        // Higher multipliers = takes longer to upgrade
        // Using SAME multipliers as wages/gold for consistency
        private const float IrregularUpgradeXpMultiplier = 1.0f;     // Base XP requirement
        private const float RegularUpgradeXpMultiplier = 1.25f;      // 25% more XP required
        private const float RangedUpgradeXpMultiplier = 1.25f;       // 25% more XP required
        private const float CrossbowUpgradeXpMultiplier = 1.0f;      // Base XP requirement (easier than ranged)
        private const float EliteUpgradeXpMultiplier = 1.5f;         // 50% more XP required
        private const float NobleUpgradeXpMultiplier = 1.5f;         // 50% more XP required

        // Cache for lowercase strings to improve performance
        private static readonly Dictionary<string, string> _lowerCaseCache = new Dictionary<string, string>();

        public override int GetGoldCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
        {
            // Get the base gold cost from vanilla calculation
            // This includes: recruitment cost difference, perks, cultural bonuses, mercenary adjustments
            int baseGoldCost = base.GetGoldCostForUpgrade(party, characterObject, upgradeTarget);

            // Apply keyword multiplier based on what the troop is upgrading TO
            float multiplier = GetKeywordMultiplier(upgradeTarget);

            // Calculate final gold cost with proper rounding
            int finalGoldCost = (int)Math.Round(baseGoldCost * multiplier, MidpointRounding.AwayFromZero);

            // Ensure cost is never negative or zero
            return Math.Max(1, finalGoldCost);
        }

        public override int GetXpCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
        {
            // Get the base XP cost from vanilla calculation
            // Vanilla uses cumulative XP based on tier differences
            int baseXpCost = base.GetXpCostForUpgrade(party, characterObject, upgradeTarget);

            // Apply keyword multiplier based on what the troop is upgrading TO
            float multiplier = GetKeywordXpMultiplier(upgradeTarget);

            // Calculate final XP cost with proper rounding
            int finalXpCost = (int)Math.Round(baseXpCost * multiplier, MidpointRounding.AwayFromZero);

            // Ensure XP cost is never negative or zero
            return Math.Max(1, finalXpCost);
        }

        /// <summary>
        /// Gets the gold cost multiplier based on keywords in the upgrade target's ID
        /// </summary>
        private float GetKeywordMultiplier(CharacterObject upgradeTarget)
        {
            // Use cached lowercase conversion
            string targetId = GetLowerCase(upgradeTarget.StringId);

            // Check in priority order: noble > elite > irregular > regular > ranged > crossbow
            // IMPORTANT: Check "irregular" before "regular" to avoid substring matching
            if (targetId.Contains("noble"))
            {
                return NobleUpgradeGoldMultiplier;
            }
            else if (targetId.Contains("elite"))
            {
                return EliteUpgradeGoldMultiplier;
            }
            else if (targetId.Contains("irregular"))
            {
                return IrregularUpgradeGoldMultiplier;
            }
            else if (targetId.Contains("regular"))
            {
                return RegularUpgradeGoldMultiplier;
            }
            else if (targetId.Contains("ranged"))
            {
                return RangedUpgradeGoldMultiplier;
            }
            else if (targetId.Contains("crossbow"))
            {
                return CrossbowUpgradeGoldMultiplier;
            }

            // Default: no multiplier for troops without keywords
            return 1.0f;
        }

        /// <summary>
        /// Gets the XP cost multiplier based on keywords in the upgrade target's ID
        /// </summary>
        private float GetKeywordXpMultiplier(CharacterObject upgradeTarget)
        {
            // Use cached lowercase conversion
            string targetId = GetLowerCase(upgradeTarget.StringId);

            // Check in priority order: noble > elite > irregular > regular > ranged > crossbow
            // IMPORTANT: Check "irregular" before "regular" to avoid substring matching
            if (targetId.Contains("noble"))
            {
                return NobleUpgradeXpMultiplier;
            }
            else if (targetId.Contains("elite"))
            {
                return EliteUpgradeXpMultiplier;
            }
            else if (targetId.Contains("irregular"))
            {
                return IrregularUpgradeXpMultiplier;
            }
            else if (targetId.Contains("regular"))
            {
                return RegularUpgradeXpMultiplier;
            }
            else if (targetId.Contains("ranged"))
            {
                return RangedUpgradeXpMultiplier;
            }
            else if (targetId.Contains("crossbow"))
            {
                return CrossbowUpgradeXpMultiplier;
            }

            // Default: no multiplier for troops without keywords
            return 1.0f;
        }

        private static string GetLowerCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if (!_lowerCaseCache.TryGetValue(input, out string result))
            {
                result = input.ToLowerInvariant();
                _lowerCaseCache[input] = result;
            }
            return result;
        }
    }
}