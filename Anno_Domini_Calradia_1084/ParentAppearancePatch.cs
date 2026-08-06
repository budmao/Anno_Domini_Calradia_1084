using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(CharacterCreationCampaignBehavior), "UpdateParentEquipment")]
    public static class ParentAppearancePatch
    {
        [HarmonyPostfix]
        public static void Postfix(CharacterCreationManager characterCreationManager)
        {
            try
            {
                string cultureId = characterCreationManager.CharacterCreationContent
                    .SelectedCulture?.StringId;
                if (cultureId == null) return;

                MBBodyProperty fatherTemplate = Game.Current.ObjectManager
                    .GetObject<MBBodyProperty>("AOM_veteran_" + cultureId);
                if (fatherTemplate == null) return;

                int race = CharacterObject.PlayerCharacter.Race;

                // Father: culture hair/beard
                BodyProperties fatherProps = BodyProperties.GetRandomBodyProperties(
                    race, false,
                    fatherTemplate.BodyPropertyMin,
                    fatherTemplate.BodyPropertyMax,
                    0, MBRandom.RandomInt(),
                    fatherTemplate.HairTags,
                    fatherTemplate.BeardTags,
                    "Cleanface,", 0f);
                fatherProps = new BodyProperties(
                    new DynamicBodyProperties(33f, 0.5f, 0.5f), fatherProps.StaticProperties);

                // Mother: female AOM template
                MBBodyProperty motherTemplate = Game.Current.ObjectManager
                    .GetObject<MBBodyProperty>("AOM_female_" + cultureId);

                BodyProperties motherProps = default;
                bool hasMother = false;

                if (motherTemplate != null)
                {
                    motherProps = BodyProperties.GetRandomBodyProperties(
                        race, true,
                        motherTemplate.BodyPropertyMin,
                        motherTemplate.BodyPropertyMax,
                        0, MBRandom.RandomInt(),
                        motherTemplate.HairTags,
                        "",
                        "Cleanface,", 0f);
                    motherProps = new BodyProperties(
                        new DynamicBodyProperties(33f, 0.3f, 0.2f), motherProps.StaticProperties);
                    hasMother = true;
                }

                foreach (NarrativeMenuCharacter character in
                    characterCreationManager.CurrentMenu.Characters)
                {
                    if (character.StringId.Equals("father_character"))
                        character.UpdateBodyProperties(fatherProps, race, false);
                    if (hasMother && character.StringId.Equals("mother_character"))
                        character.UpdateBodyProperties(motherProps, race, true);
                }
            }
            catch (System.Exception ex)
            {
                Main.Log($"[ParentAppearance] Error: {ex}");
            }
        }
    }
}