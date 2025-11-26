using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;

namespace Anno_Domini_Calradia_1084
{
    public class WageModel : DefaultPartyWageModel
    {
        // Wage multipliers - adjust these values as needed
        private const float IrregularWageMultiplier = 1.0f;   // Base cost
        private const float RegularWageMultiplier = 1.25f;    // 25% more expensive
        private const float RangedWageMultiplier = 1.25f;     // 25% more expensive
        private const float CrossbowWageMultiplier = 1.25f;   // 25% more expensive (same as ranged)
        private const float EliteWageMultiplier = 1.5f;       // 50% more expensive
        private const float NobleWageMultiplier = 1.5f;       // 50% more expensive
        private const float MercenaryWageMultiplier = 1.75f;  // 75% more expensive (replaces vanilla 1.5x)

        // Cache for lowercase strings to improve performance
        private static readonly Dictionary<string, string> _lowerCaseCache = new Dictionary<string, string>();

        public override int GetCharacterWage(CharacterObject character)
        {
            // Start with tier-based wage (without the default mercenary bonus)
            int num;
            switch (character.Tier)
            {
                case 0: num = 1; break;
                case 1: num = 2; break;
                case 2: num = 3; break;
                case 3: num = 5; break;
                case 4: num = 8; break;
                case 5: num = 12; break;
                case 6: num = 17; break;
                default: num = 23; break;
            }

            int baseWage = num;

            // Get character ID in lowercase for case-insensitive matching
            string characterId = GetLowerCase(character.StringId);

            // Apply multiplier based on keywords
            // Priority order: noble > elite > irregular > regular > ranged > crossbow
            // IMPORTANT: Check "irregular" before "regular" to avoid substring matching!
            float multiplier = 1.0f;

            if (characterId.Contains("noble"))
            {
                multiplier = NobleWageMultiplier;
            }
            else if (characterId.Contains("elite"))
            {
                multiplier = EliteWageMultiplier;
            }
            else if (characterId.Contains("irregular"))  // Check BEFORE "regular"!
            {
                multiplier = IrregularWageMultiplier;
            }
            else if (characterId.Contains("regular"))
            {
                multiplier = RegularWageMultiplier;
            }
            else if (characterId.Contains("ranged"))
            {
                multiplier = RangedWageMultiplier;
            }
            else if (characterId.Contains("crossbow"))
            {
                multiplier = CrossbowWageMultiplier;
            }

            // Apply keyword multiplier
            int wage = (int)Math.Round(baseWage * multiplier, MidpointRounding.AwayFromZero);

            // Apply custom mercenary multiplier (overrides vanilla 1.5x)
            if (character.Occupation == Occupation.Mercenary)
            {
                wage = (int)Math.Round(wage * MercenaryWageMultiplier, MidpointRounding.AwayFromZero);
            }

            return wage;
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