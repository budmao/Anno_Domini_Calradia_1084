using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SandBox.CampaignBehaviors;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace Anno_Domini_Calradia_1084
{
    public class CitizenSpawnPatch
    {
        // Ratios: fraction of scene spawn points to fill with NPCs (0.0 = off, 1.0 = all points).
        // Minimums: guaranteed NPC count regardless of formula (scene must have spawn points).
        // Rate multipliers: scale the native spawn rate (time-of-day × prosperity × weather).
        // Rate floor: minimum spawn rate for castles regardless of conditions.

        // ── Castle day ────────────────────────────────────────────
        private const float CastleMenRatio = 0.25f;
        private const float CastleWomenRatio = 0.0f;
        private const int CastleMinMen = 5;
        private const int CastleMinWomen = 0;

        // ── Castle night (fewer people, all carry torches) ────────
        private const float CastleNightMenRatio = 0.15f;
        private const float CastleNightWomenRatio = 0.0f;
        private const int CastleNightMinMen = 5;
        private const int CastleNightMinWomen = 0;

        // ── Town/Village night (fewer people, all carry torches) ────────
        private const float TownNightRateMultiplier = 1.0f;
        private const float VillageNightRateMultiplier = 0.8f;
        private const float TownNightRateFloor = 0.05f;
        private const float VillageNightRateFloor = 0.01f;

        // ── Tavern extras ─────────────────────────────────────────
        private const float TavernExtraMenRatio = 0.2f;
        private const float TavernExtraWomenRatio = 0.1f;

        // ── Rate multipliers ──────────────────────────────────────
        private const float CastleRateMultiplier = 2.0f;
        private const float CastleRateFloor = 0.15f;
        private const float TownRateMultiplier = 2.5f;
        private const float VillageRateMultiplier = 2.5f;

        // ── Cached reflection handles ─────────────────────────────

        private static readonly MethodInfo GetSpawnRateMethod =
            AccessTools.Method(typeof(CommonTownsfolkCampaignBehavior), "GetSpawnRate");

        private static readonly MethodInfo CreateTownsManMethod =
            AccessTools.Method(typeof(CommonTownsfolkCampaignBehavior), "CreateTownsMan");

        private static readonly MethodInfo CreateTownsWomanMethod =
            AccessTools.Method(typeof(CommonTownsfolkCampaignBehavior), "CreateTownsWoman");

        private static readonly MethodInfo CreateTownsManForTavernMethod =
            AccessTools.Method(typeof(CommonTownsfolkCampaignBehavior), "CreateTownsManForTavern");

        private static readonly MethodInfo CreateTownsWomanForTavernMethod =
            AccessTools.Method(typeof(CommonTownsfolkCampaignBehavior), "CreateTownsWomanForTavern");

        private static CreateLocationCharacterDelegate _townsManDelegate;
        private static CreateLocationCharacterDelegate _townsWomanDelegate;
        private static CreateLocationCharacterDelegate _tavernManDelegate;
        private static CreateLocationCharacterDelegate _tavernWomanDelegate;

        private static CreateLocationCharacterDelegate TownsManDelegate
        {
            get
            {
                if (_townsManDelegate == null && CreateTownsManMethod != null)
                    _townsManDelegate = (CreateLocationCharacterDelegate)
                        Delegate.CreateDelegate(typeof(CreateLocationCharacterDelegate), CreateTownsManMethod);
                return _townsManDelegate;
            }
        }

        private static CreateLocationCharacterDelegate TownsWomanDelegate
        {
            get
            {
                if (_townsWomanDelegate == null && CreateTownsWomanMethod != null)
                    _townsWomanDelegate = (CreateLocationCharacterDelegate)
                        Delegate.CreateDelegate(typeof(CreateLocationCharacterDelegate), CreateTownsWomanMethod);
                return _townsWomanDelegate;
            }
        }

        private static CreateLocationCharacterDelegate TavernManDelegate
        {
            get
            {
                if (_tavernManDelegate == null && CreateTownsManForTavernMethod != null)
                    _tavernManDelegate = (CreateLocationCharacterDelegate)
                        Delegate.CreateDelegate(typeof(CreateLocationCharacterDelegate), CreateTownsManForTavernMethod);
                return _tavernManDelegate;
            }
        }

        private static CreateLocationCharacterDelegate TavernWomanDelegate
        {
            get
            {
                if (_tavernWomanDelegate == null && CreateTownsWomanForTavernMethod != null)
                    _tavernWomanDelegate = (CreateLocationCharacterDelegate)
                        Delegate.CreateDelegate(typeof(CreateLocationCharacterDelegate), CreateTownsWomanForTavernMethod);
                return _tavernWomanDelegate;
            }
        }

        // ── Patch 1: Spawn townsfolk in castles ───────────────────
        //
        // The native method gates everything behind if (!settlement.IsCastle),
        // then looks up locations by town-specific IDs ("center", "tavern").
        // Castle LocationComplexes don't have those IDs, so we let the native
        // code skip castles and handle them entirely in this postfix.
        // Day and night use separate ratios and minimums.

        [HarmonyPatch(typeof(CommonTownsfolkCampaignBehavior), "LocationCharactersAreReadyToSpawn")]
        public static class LocationCharactersAreReadyToSpawnPatch
        {
            static void Postfix(CommonTownsfolkCampaignBehavior __instance,
                                Dictionary<string, int> unusedUsablePointCount)
            {
                try
                {
                    Settlement settlement = PlayerEncounter.LocationEncounter?.Settlement;
                    if (settlement == null || !settlement.IsCastle)
                        return;

                    Location currentLocation = CampaignMission.Current?.Location;
                    if (currentLocation == null || currentLocation.StringId == "prison")
                        return;

                    unusedUsablePointCount.TryGetValue("npc_common", out int npcCommon);
                    unusedUsablePointCount.TryGetValue("npc_common_limited", out int npcLimited);
                    int totalPoints = npcCommon + npcLimited;

                    if (totalPoints == 0)
                    {
                        //Main.Log($"[CitizenSpawn] Castle '{settlement.Name}' has no npc_common points — skipping.");
                        return;
                    }

                    float currentHour = CampaignTime.Now.CurrentHourInDay;
                    bool isNight = currentHour < 6f || currentHour >= 20f;
                    float menRatio = isNight ? CastleNightMenRatio : CastleMenRatio;
                    float womenRatio = isNight ? CastleNightWomenRatio : CastleWomenRatio;
                    int minMen = isNight ? CastleNightMinMen : CastleMinMen;
                    int minWomen = isNight ? CastleNightMinWomen : CastleMinWomen;

                    float spawnRate = (float)GetSpawnRateMethod.Invoke(__instance, new object[] { settlement });
                    CultureObject culture = settlement.Culture;

                    if (menRatio > 0f || minMen > 0)
                    {
                        int menCount = Math.Max(minMen, (int)(totalPoints * menRatio * spawnRate));
                        var del = TownsManDelegate;
                        if (del != null && menCount > 0)
                            currentLocation.AddLocationCharacters(
                                del, culture, LocationCharacter.CharacterRelations.Neutral, menCount);
                    }

                    if (womenRatio > 0f || minWomen > 0)
                    {
                        int womenCount = Math.Max(minWomen, (int)(totalPoints * womenRatio * spawnRate));
                        var del = TownsWomanDelegate;
                        if (del != null && womenCount > 0)
                            currentLocation.AddLocationCharacters(
                                del, culture, LocationCharacter.CharacterRelations.Neutral, womenCount);
                    }
                }
                catch (Exception ex)
                {
                    Main.Log($"[CitizenSpawn] Castle error: {ex}");
                }
            }
        }

        // ── Patch 2: Extra tavern spawns ──────────────────────────
        //
        // Adds additional townsfolk on top of what vanilla already
        // places in the tavern, using the tavern-specific character
        // creators (seated/idle animations instead of outdoor wandering).

        [HarmonyPatch(typeof(CommonTownsfolkCampaignBehavior), "AddPeopleToTownTavern")]
        public static class AddPeopleToTavernPatch
        {
            static void Postfix(Settlement settlement, Dictionary<string, int> unusedUsablePointCount)
            {
                try
                {
                    unusedUsablePointCount.TryGetValue("npc_common", out int npcCommon);
                    if (npcCommon <= 0)
                        return;

                    Location tavern = settlement.LocationComplex.GetLocationWithId("tavern");
                    if (tavern == null)
                        return;

                    CultureObject culture = settlement.Culture;

                    if (TavernExtraMenRatio > 0f)
                    {
                        int menCount = (int)(npcCommon * TavernExtraMenRatio);
                        var del = TavernManDelegate;
                        if (del != null && menCount > 0)
                            tavern.AddLocationCharacters(
                                del, culture, LocationCharacter.CharacterRelations.Neutral, menCount);
                    }

                    if (TavernExtraWomenRatio > 0f)
                    {
                        int womenCount = (int)(npcCommon * TavernExtraWomenRatio);
                        var del = TavernWomanDelegate;
                        if (del != null && womenCount > 0)
                            tavern.AddLocationCharacters(
                                del, culture, LocationCharacter.CharacterRelations.Neutral, womenCount);
                    }
                }
                catch (Exception ex)
                {
                    Main.Log($"[CitizenSpawn] Tavern error: {ex}");
                }
            }
        }

        // ── Patch 3: Boost town & castle spawn rates ──────────────

        [HarmonyPatch(typeof(CommonTownsfolkCampaignBehavior), "GetSpawnRate")]
        public static class GetSpawnRatePatch
        {
            static void Postfix(Settlement settlement, ref float __result)
            {
                if (settlement.IsCastle)
                {
                    __result *= CastleRateMultiplier;
                    if (__result < CastleRateFloor && __result > 0.001f)
                        __result = CastleRateFloor;
                }
                else if (settlement.IsTown)
                {
                    float hour = CampaignTime.Now.CurrentHourInDay;
                    bool isNight = hour < 6f || hour >= 20f;
                    __result *= isNight ? TownNightRateMultiplier : TownRateMultiplier;
                    if (isNight && __result < TownNightRateFloor && __result > 0.001f)
                        __result = TownNightRateFloor;
                }
            }
        }

        // ── Patch 4: Boost village spawn rate ─────────────────────
        //
        // Villages use CommonVillagersCampaignBehavior with their own
        // GetSpawnRate, so they need a separate patch.

        [HarmonyPatch(typeof(CommonVillagersCampaignBehavior), "GetSpawnRate")]
        public static class VillageGetSpawnRatePatch
        {
            static void Postfix(Settlement settlement, ref float __result)
            {
                if (settlement.IsVillage)
                {
                    float hour = CampaignTime.Now.CurrentHourInDay;
                    bool isNight = hour < 6f || hour >= 20f;
                    __result *= isNight ? VillageNightRateMultiplier : VillageRateMultiplier;
                    if (isNight && __result < VillageNightRateFloor && __result > 0.001f)
                        __result = VillageNightRateFloor;
                }
            }
        }

        // ── Patch 5: Force torches on castle civilians at night ───
        //
        // The native spawn system only gives torches to NPCs that land
        // on spawn points tagged "torch". This prefix forces hasTorch
        // for all non-hero, non-soldier NPCs in castles at night, so
        // the native code handles equipment cloning and wielding.

        [HarmonyPatch(typeof(MissionAgentHandler), "SpawnWanderingAgentWithInitialFrame")]
        public static class CastleNightTorchPatch
        {
            static void Prefix(LocationCharacter locationCharacter, ref bool hasTorch)
            {
                try
                {
                    if (hasTorch)
                        return;

                    float hour = CampaignTime.Now.CurrentHourInDay;
                    if (hour >= 6f && hour < 20f)
                        return;

                    Settlement settlement = PlayerEncounter.LocationEncounter?.Settlement;
                    if (settlement == null)
                        return;

                    Location location = CampaignMission.Current?.Location;
                    if (location == null
                        || location.StringId == "tavern"
                        || location.StringId == "lordshall"
                        || location.StringId == "prison")
                        return;

                    if (locationCharacter.Character.IsHero
                        || locationCharacter.Character.IsSoldier
                        || locationCharacter.Character.StringId.Contains("prison_guard")
                        || locationCharacter.Character.Occupation == Occupation.Gangster
                        || locationCharacter.SpecialTargetTag != "npc_common")
                        return;

                    hasTorch = true;

                    //Main.DebugLog($"[CitizenSpawn] Torch given to '{locationCharacter.Character.Name}' actionSet={locationCharacter.ActionSetCode}");
                }
                catch { }
            }
        }
    }
}