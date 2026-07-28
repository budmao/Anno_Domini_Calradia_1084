using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace Anno_Domini_Calradia_1084.Patches
{
    // Applies random negative modifiers to player starting gear
    // after character creation is finalized.
    //
    // Patches CharacterCreationState.FinalizeCharacterCreationState —
    // the actual method that transitions the player to the campaign map.
    // By the time our Postfix runs, equipment is fully assigned,
    // the MapState is pushed, visuals are marked dirty, and
    // OnCharacterCreationIsOver has fired.
    [HarmonyPatch(typeof(CharacterCreationState))]
    [HarmonyPatch("FinalizeCharacterCreationState")]
    public static class StartingGearPatch
    {
        // Set to true to enable detailed logging, false for release
        private const bool DebugEnabled = false;

        // Chance that any given slot gets a modifier at all (skip some for variety)
        private const float ChanceToApply = 0.75f;

        // Among applied modifiers: 65% inferior, 35% poor
        private const float InferiorWeight = 0.65f;

        [HarmonyPostfix]
        public static void Postfix()
        {
            try
            {
                Hero player = Hero.MainHero;
                if (player == null)
                {
                    if (DebugEnabled) Main.DebugLog("[StartingGear] MainHero is null, skipping.");
                    return;
                }

                // Build lookup of negative modifiers grouped by ItemModifierGroup.
                // Uses the engine's own GetModifiersBasedOnQuality() API.
                Dictionary<ItemModifierGroup, ModifiersByQuality> groupedModifiers = BuildModifierLookup();
                if (DebugEnabled) Main.DebugLog($"[StartingGear] Found {groupedModifiers.Count} modifier group(s) with negative modifiers.");

                if (groupedModifiers.Count == 0)
                {
                    if (DebugEnabled) Main.DebugLog("[StartingGear] No negative modifiers found in any group. Aborting.");
                    return;
                }

                Equipment battleEquip = player.BattleEquipment;
                if (battleEquip != null)
                {
                    ApplyNegativeModifiers(battleEquip, groupedModifiers, "Battle");
                }

                Equipment civilianEquip = player.CivilianEquipment;
                if (civilianEquip != null)
                {
                    ApplyNegativeModifiers(civilianEquip, groupedModifiers, "Civilian");
                }

                // Hero.StealthEquipment — falls back to DefaultStealthEquipment if private field is null
                Equipment stealthEquip = player.StealthEquipment;
                if (stealthEquip != null)
                {
                    ApplyNegativeModifiers(stealthEquip, groupedModifiers, "Stealth");
                }
            }
            catch (Exception ex)
            {
                Main.Log($"[StartingGear] Error applying modifiers: {ex}");
            }
        }

        private static void ApplyNegativeModifiers(
            Equipment equipment,
            Dictionary<ItemModifierGroup, ModifiersByQuality> groupedModifiers,
            string equipmentLabel)
        {
            for (int i = 0; i < (int)EquipmentIndex.NumEquipmentSetSlots; i++)
            {
                EquipmentIndex index = (EquipmentIndex)i;
                EquipmentElement element = equipment[index];

                if (element.Item == null)
                    continue;

                if (element.ItemModifier != null)
                {
                    if (DebugEnabled) Main.DebugLog($"[StartingGear] {equipmentLabel}[{index}] '{element.Item.StringId}' already modified, skipping.");
                    continue;
                }

                // Random chance to skip this slot (variety)
                if (MBRandom.RandomFloat > ChanceToApply)
                {
                    if (DebugEnabled) Main.DebugLog($"[StartingGear] {equipmentLabel}[{index}] '{element.Item.StringId}' skipped by chance.");
                    continue;
                }

                ItemModifier modifier = GetRandomNegativeModifier(element.Item, groupedModifiers);
                if (modifier != null)
                {
                    // Build a new EquipmentElement with the modifier baked in.
                    // EquipmentElement is a struct — must replace the whole slot.
                    // Matches how Hero.HandleInvalidModifier does it.
                    equipment[index] = new EquipmentElement(element.Item, modifier, null, false);
                    if (DebugEnabled) Main.DebugLog($"[StartingGear] {equipmentLabel}[{index}] '{element.Item.StringId}' -> '{modifier.StringId}' ({modifier.ItemQuality})");
                }
                else
                {
                    if (DebugEnabled) Main.DebugLog($"[StartingGear] {equipmentLabel}[{index}] '{element.Item.StringId}' — no matching negative modifier found.");
                }
            }
        }

        private static ItemModifier GetRandomNegativeModifier(
            ItemObject item,
            Dictionary<ItemModifierGroup, ModifiersByQuality> groupedModifiers)
        {
            // 1. Try the item's own modifier group first (most type-accurate)
            ItemModifierGroup itemGroup = item.ItemComponent?.ItemModifierGroup;
            if (itemGroup != null && groupedModifiers.TryGetValue(itemGroup, out ModifiersByQuality direct))
            {
                if (DebugEnabled) Main.DebugLog($"[StartingGear]   '{item.StringId}' matched own group '{itemGroup.StringId}'");
                return PickFromQuality(direct);
            }

            // 2. Fallback: pick from all available negative modifiers
            if (DebugEnabled) Main.DebugLog($"[StartingGear]   '{item.StringId}' has no group, using fallback.");
            return PickFromAllGroups(groupedModifiers);
        }

        private static ItemModifier PickFromQuality(ModifiersByQuality mods)
        {
            float roll = MBRandom.RandomFloat;

            if (roll < InferiorWeight && mods.Inferior.Count > 0)
                return mods.Inferior[MBRandom.RandomInt(mods.Inferior.Count)];

            if (mods.Poor.Count > 0)
                return mods.Poor[MBRandom.RandomInt(mods.Poor.Count)];

            if (mods.Inferior.Count > 0)
                return mods.Inferior[MBRandom.RandomInt(mods.Inferior.Count)];

            return null;
        }

        private static ItemModifier PickFromAllGroups(
            Dictionary<ItemModifierGroup, ModifiersByQuality> groupedModifiers)
        {
            List<ItemModifier> allInferior = new List<ItemModifier>();
            List<ItemModifier> allPoor = new List<ItemModifier>();

            foreach (ModifiersByQuality mods in groupedModifiers.Values)
            {
                allInferior.AddRange(mods.Inferior);
                allPoor.AddRange(mods.Poor);
            }

            float roll = MBRandom.RandomFloat;

            if (roll < InferiorWeight && allInferior.Count > 0)
                return allInferior[MBRandom.RandomInt(allInferior.Count)];

            if (allPoor.Count > 0)
                return allPoor[MBRandom.RandomInt(allPoor.Count)];

            if (allInferior.Count > 0)
                return allInferior[MBRandom.RandomInt(allInferior.Count)];

            return null;
        }

        /// <summary>
        /// Scans all registered ItemModifierGroups and collects their
        /// Inferior/Poor modifiers using the engine's own
        /// GetModifiersBasedOnQuality() API.
        /// </summary>
        private static Dictionary<ItemModifierGroup, ModifiersByQuality> BuildModifierLookup()
        {
            var result = new Dictionary<ItemModifierGroup, ModifiersByQuality>();

            MBReadOnlyList<ItemModifierGroup> allGroups =
                MBObjectManager.Instance.GetObjectTypeList<ItemModifierGroup>();

            if (allGroups == null)
                return result;

            foreach (ItemModifierGroup group in allGroups)
            {
                if (group == null)
                    continue;

                // Use the engine's own quality filter
                List<ItemModifier> inferior = group.GetModifiersBasedOnQuality(ItemQuality.Inferior);
                List<ItemModifier> poor = group.GetModifiersBasedOnQuality(ItemQuality.Poor);

                if (inferior.Count > 0 || poor.Count > 0)
                {
                    result[group] = new ModifiersByQuality
                    {
                        Inferior = inferior,
                        Poor = poor
                    };
                    if (DebugEnabled) Main.DebugLog($"[StartingGear] Group '{group.StringId}': {inferior.Count} inferior, {poor.Count} poor.");
                }
            }

            return result;
        }

        private class ModifiersByQuality
        {
            public List<ItemModifier> Inferior = new List<ItemModifier>();
            public List<ItemModifier> Poor = new List<ItemModifier>();
        }
    }
}