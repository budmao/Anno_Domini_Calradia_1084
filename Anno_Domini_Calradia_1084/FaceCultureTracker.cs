using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace Anno_Domini_Calradia_1084
{
    // ==================================================================================
    // Culture tracker — maintains a parallel list of cultures for each cached face,
    // plus a per-mission salt to force unique seeds across battles.
    //
    // MissionFaceCacheView._alreadyAssignedFaces stores cached faces but has NO concept
    // of culture. In vanilla Bannerlord all troops are race 0 (human), so the race check
    // in ComputeSimilarityOfFace (10f penalty) never triggers. The actual visual diversity
    // between factions (Khuzait, Battanian, Aserai, etc.) comes entirely from different
    // BodyProperties min/max ranges per culture — but the cache doesn't track this.
    //
    // When a cached Khuzait face is "similar enough" to a new Battanian face in
    // FaceGenerationParams space, the cache reuses the Khuzait's StaticBodyProperties
    // (skin tone, face shape) for the Battanian → faction swap bug.
    //
    // We track which culture each cache entry belongs to and reject cross-culture matches.
    // ==================================================================================

    internal static class FaceCultureTracker
    {
        internal static readonly List<BasicCultureObject> CachedCultures = new List<BasicCultureObject>();
        internal static BasicCultureObject CurrentCulture;

        // Per-mission salt XOR'd into face seeds to prevent the native engine from
        // returning stale cached results across battles (clone face / "adam" bug).
        internal static int MissionSalt;

        internal static void Clear()
        {
            CachedCultures.Clear();
            CurrentCulture = null;
        }

        internal static void NewMission()
        {
            Clear();
            // Generate a unique salt for this mission using high-resolution time.
            // This ensures the native GetRandomBodyProperties receives different seeds
            // each battle, even if its internal cache persists across missions.
            MissionSalt = Environment.TickCount ^ (int)(Time.ApplicationTime * 1000f);
        }
    }

    // ==================================================================================
    // FIX 1: Flush native face mesh cache + reset tracker + new salt at mission start
    //
    // The native engine cache (MBAPI.IMBFaceGen) persists across missions and is only
    // flushed in OnMissionScreenFinalize. Due to timing between screen finalization and
    // new mission initialization, stale mesh data can persist.
    //
    // Additionally, even after flushing, the native GetRandomBodyProperties may have
    // internal state that causes it to return identical results for the same seeds.
    // The mission salt ensures every battle uses unique seeds, bypassing this entirely.
    // ==================================================================================

    [HarmonyPatch(typeof(MissionFaceCacheView), "OnBehaviorInitialize")]
    public static class Patch_FlushCacheOnMissionStart
    {
        public static void Prefix()
        {
            // Use the exact same call as OnMissionScreenFinalize:
            // TaleWorlds.MountAndBlade.FaceGen.FlushFaceCache()
            // (not MBBodyProperties — to flush the correct cache in the correct assembly)
            TaleWorlds.MountAndBlade.FaceGen.FlushFaceCache();
            MBBodyProperties.FlushFaceCache();
            FaceCultureTracker.NewMission();
        }
    }

    // ==================================================================================
    // Clean up tracker when mission ends
    // ==================================================================================

    [HarmonyPatch(typeof(MissionFaceCacheView), "OnMissionScreenFinalize")]
    public static class Patch_ClearTrackerOnFinalize
    {
        public static void Prefix()
        {
            FaceCultureTracker.Clear();
        }
    }

    // ==================================================================================
    // FIX 2: Track culture + inject mission-unique seed entropy
    //
    // GetRandomBodyPropertyForTroop receives the BasicCharacterObject (which has .Culture)
    // but passes only FaceGenerationParams to the cache system, losing culture info.
    //
    // Prefix:
    //   - Capture the current character's culture before the cache lookup.
    //   - XOR the face seed with a per-mission salt so the native engine generates
    //     fresh faces each battle instead of returning stale cached results.
    //
    // Postfix:
    //   - If a new unique face was added to the cache, record its culture.
    // ==================================================================================

    [HarmonyPatch(typeof(MissionFaceCacheView), "GetRandomBodyPropertyForTroop")]
    public static class Patch_TrackFaceCulture
    {
        private static readonly FieldInfo UniqueIndexField =
            AccessTools.Field(typeof(MissionFaceCacheView), "_uniqueCacheIndex");

        public static void Prefix(BasicCharacterObject characterObject, ref int seed)
        {
            FaceCultureTracker.CurrentCulture = characterObject?.Culture;

            // Inject mission-unique entropy into the seed.
            // The native MBAPI.IMBFaceGen.GetRandomBodyProperties may cache results
            // keyed by seed. By XOR'ing with a per-mission salt, we guarantee different
            // seeds each battle → different native face generation results → no clones.
            seed = seed ^ FaceCultureTracker.MissionSalt;
        }

        public static void Postfix(MissionFaceCacheView __instance)
        {
            int cacheCount = (int)UniqueIndexField.GetValue(__instance);
            while (FaceCultureTracker.CachedCultures.Count < cacheCount)
            {
                FaceCultureTracker.CachedCultures.Add(FaceCultureTracker.CurrentCulture);
            }
        }
    }

    // ==================================================================================
    // FIX 3: Reject cross-culture cache matches
    //
    // CheckForSimilarFacesFromCache returns an index into the cache when it finds a
    // "similar enough" face. If that cached face belongs to a different culture, we
    // override the result to -1, forcing a new unique face to be generated.
    //
    // This may slightly exceed the face mesh budget in mixed-culture battles, but
    // visual correctness (Battanians looking like Battanians) is more important than
    // the memory optimization. In practice, same-culture faces will still match and
    // share meshes, keeping memory usage reasonable.
    // ==================================================================================

    [HarmonyPatch(typeof(MissionFaceCacheView), "CheckForSimilarFacesFromCache")]
    public static class Patch_PreventCrossCultureReuse
    {
        public static void Postfix(ref int __result)
        {
            if (__result < 0)
                return; // Already creating new face, nothing to check

            if (__result < FaceCultureTracker.CachedCultures.Count)
            {
                BasicCultureObject cachedCulture = FaceCultureTracker.CachedCultures[__result];
                if (cachedCulture != FaceCultureTracker.CurrentCulture)
                {
                    // Cached face is from a different culture — reject it.
                    // GetRandomBodyPropertyForTroop will create a new unique face instead.
                    __result = -1;
                }
            }
        }
    }

    // ==================================================================================
    // FIX 4: Increase race mismatch penalty (for mods with custom races)
    //
    // This doesn't affect vanilla (all troops are race 0), but mods like TOR that add
    // custom races (orcs, elves, etc.) benefit from the race penalty being increased
    // from 10 to 1,000,000. This is a safety net on top of the culture-aware fix above.
    // ==================================================================================

    [HarmonyPatch(typeof(MissionFaceCacheView), "ComputeSimilarityOfFace")]
    public static class Patch_RacePenaltyForMods
    {
        public static void Postfix(
            ref float __result,
            FaceGenerationParams f0,
            FaceGenerationParams f1)
        {
            if (f0.CurrentRace != f1.CurrentRace)
            {
                // Original penalty is 10f (already included in __result).
                // Add 999,990 more to reach 1,000,000 total — matching gender/haircover weight.
                __result += 999990f;
            }
        }
    }


    // ==================================================================================
    // FIX 5: Disable native face mesh cache
    //
    // The engine has a SECOND cache layer: Agent.EquipItemsFromSpawnEquipment uses
    // FaceCacheId to look up pre-built face meshes in a native cache. This cache
    // persists across missions and is NOT cleared by FlushFaceCache().
    // When battle 2 reuses CacheID 0, the engine serves battle 1's mesh → "adam".
    //
    // Disabling this cache forces the engine to build fresh face meshes from the
    // BodyProperties each time. The managed cache in MissionFaceCacheView still
    // provides memory optimization by sharing BodyProperties between similar troops.
    // ==================================================================================

    [HarmonyPatch(typeof(Agent), nameof(Agent.EquipItemsFromSpawnEquipment))]
    public static class Patch_DisableNativeFaceMeshCache
    {
        public static void Prefix(ref bool useFaceCache)
        {
            useFaceCache = false;
        }
    }

}