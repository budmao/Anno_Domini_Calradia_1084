using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Localization;

namespace CharacterCreationRedone.VanillaOptions
{
    [HarmonyPatch(typeof(CharacterCreationCampaignBehavior), nameof(CharacterCreationCampaignBehavior.InitializeData))]
    public class CharacterCreationRedoneVanilla : CharacterCreationCampaignBehavior, ICharacterCreationContentHandler
    {
        // these are excluded from the project and is intended for modders to use in their mod with losing countless hours everytime
        [HarmonyPrefix]
        static bool Prefix(ref CharacterCreationRedoneVanilla __instance, CharacterCreationManager characterCreationManager)
        {
            characterCreationManager.CharacterCreationContent.ChangeReviewPageDescription(new TextObject("{=W6pKpEoT}You prepare to set off for a grand adventure in Calradia! Here is your character. Continue if you are ready, or go back to make changes.", null));
            var ParentsMenu = new CharacterCreationRedoneVanillaParentsMenu();
            var ChildhoodMenu = new CharacterCreationRedoneVanillaChildhoodMenu();
            var EducationMenu = new CharacterCreationRedoneVanillaEducationMenu();
            var youthmenu = new CharacterCreationRedoneVanillaYouthMenu();
            var AdulthoodMenu = new CharacterCreationRedoneVanillaAdulthoodMenu();
            var AgeMenu = new CharacterCreationRedoneVanillaAgeMenu();
            ParentsMenu.AddParentsMenu(characterCreationManager);
            ChildhoodMenu.AddChildhoodMenu(characterCreationManager);
            EducationMenu.AddEducationMenu(characterCreationManager);
            youthmenu.AddYouthMenu(characterCreationManager);
            AdulthoodMenu.AddAdulthoodMenu(characterCreationManager);
            AgeMenu.AddAgeSelectionMenu(characterCreationManager);
            return false;
        }
    }
}