using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;

namespace Anno_Domini_Calradia_1084.Patches
{
    // Set to true to enable [LordFace] debug logging
    public static class LordFaceConfig
    {
        public const bool DebugLog = false;
    }
    /// <summary>
    /// When lords are loaded from XML via Hero.Deserialize, they only take
    /// GetBodyPropertiesMin — no randomization, no hair/beard selection.
    /// This patch detects lords that use our AOM face_key_template (assigned
    /// via XSLT) and generates a proper randomized face from the template
    /// range, including hair, beard, and tattoo tags.
    /// </summary>
    [HarmonyPatch(typeof(Hero), "Deserialize")]
    public static class HeroLordFaceRandomizationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Hero __instance)
        {
            try
            {
                CharacterObject character = __instance.CharacterObject;
                if (character == null) return;

                MBBodyProperty bodyPropertyRange = character.BodyPropertyRange;
                if (bodyPropertyRange == null) return;

                // Only process characters that use our AOM face_key_template.
                // Rulers and other lords excluded from the XSLT keep their
                // unique BodyProperties and are not affected.
                if (!bodyPropertyRange.StringId.StartsWith("AOM_")) return;

                BodyProperties min = character.GetBodyPropertiesMin(false);
                BodyProperties max = character.GetBodyPropertiesMax(false);

                // Deterministic seed from the character's ID so each lord
                // gets a unique but consistent face across loads.
                int seed = character.GetDefaultFaceSeed(0);

                // Override tattoo tags for lords.
                // Female lords always get Cleanface.
                // Male lords: 60% clean face, 40% scar.
                string lordTattooTags;
                if (character.IsFemale)
                {
                    lordTattooTags = "Cleanface,";
                }
                else
                {
                    int tattooRoll = (seed * 31 + character.StringId.Length) % 100;
                    if (tattooRoll < 60)
                    {
                        lordTattooTags = "Cleanface,";
                    }
                    else
                    {
                        lordTattooTags = "Scar1,Scar2,Scar3,Scar4,Scar5,Scar6,Scar7,Scar8,Scar9,Scar10,Scar11,Scar13,Scar16,Scar17,";
                    }
                }

                BodyProperties randomProps = BodyProperties.GetRandomBodyProperties(
                    character.Race,
                    character.IsFemale,
                    min,
                    max,
                    0, // hairCoverType — no helmet during creation
                    seed,
                    bodyPropertyRange.HairTags,
                    bodyPropertyRange.BeardTags,
                    lordTattooTags,
                    0f
                );

                __instance.StaticBodyProperties = randomProps.StaticProperties;
                __instance.Weight = randomProps.DynamicProperties.Weight;
                __instance.Build = randomProps.DynamicProperties.Build;

                if (LordFaceConfig.DebugLog)
                {
                    Main.Log($"[LordFace] Randomized face for {character.StringId} using {bodyPropertyRange.StringId}");
                }
            }
            catch (System.Exception ex)
            {
                Main.Log($"[LordFace] Error randomizing face for {__instance?.CharacterObject?.StringId}: {ex}");
            }
        }
    }

    /// <summary>
    /// Handles scar ratio for runtime-created lords (offspring, template heroes).
    /// These go through GetStaticBodyProperties instead of Hero.Deserialize.
    /// After the original method generates the face, this postfix checks if
    /// the hero is a male lord using AOM templates and applies the 60/40
    /// clean/scar ratio by overriding the tattoo via FaceGen.SetHair.
    /// 
    /// NOTE: If the concrete class name differs in your build, update the
    /// typeof() to match (e.g. SandboxHeroCreationModel).
    /// </summary>
    [HarmonyPatch(typeof(DefaultHeroCreationModel), "GetStaticBodyProperties")]
    public static class HeroOffspringScarRatioPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref StaticBodyProperties __result, Hero hero, bool isOffspring)
        {
            try
            {
                if (hero == null) return;

                bool usesAom = false;

                if (isOffspring)
                {
                    // Check same-gender parent's template
                    Hero parent = hero.IsFemale ? hero.Mother : hero.Father;
                    MBBodyProperty parentRange = parent?.CharacterObject?.BodyPropertyRange;
                    if (parentRange != null && parentRange.StringId.StartsWith("AOM_"))
                        usesAom = true;
                }
                else if (!hero.CharacterObject.IsOriginalCharacter)
                {
                    // Template-created hero: check original template
                    CharacterObject original = hero.CharacterObject.OriginalCharacter;
                    MBBodyProperty originalRange = original?.BodyPropertyRange;
                    if (originalRange != null && originalRange.StringId.StartsWith("AOM_"))
                        usesAom = true;
                }

                if (!usesAom) return;

                // 60/40 clean/scar ratio for males, always clean for females
                int hash = hero.CharacterObject.StringId.GetHashCode();
                int roll = ((hash >= 0 ? hash : -hash) * 31) % 100;

                if (hero.IsFemale || roll < 60)
                {
                    // Force cleanface — tattoo index 0 = Cleanface
                    BodyProperties props = new BodyProperties(
                        new DynamicBodyProperties(hero.Age, hero.Weight, hero.Build),
                        __result);
                    FaceGen.SetHair(ref props, -1, -1, 0);
                    __result = props.StaticProperties;
                }

                if (LordFaceConfig.DebugLog)
                {
                    string type = isOffspring ? "offspring" : "template";
                    Main.Log($"[LordFace] Scar ratio applied to {type} {hero.CharacterObject.StringId} (roll={roll}, clean={roll < 60})");
                }
            }
            catch (System.Exception ex)
            {
                Main.Log($"[LordFace] Error in scar ratio for {hero?.CharacterObject?.StringId}: {ex}");
            }
        }
    }
}