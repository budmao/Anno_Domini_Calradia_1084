using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Localization;

namespace Anno_Domini_Calradia_1084
{
    [HarmonyPatch(typeof(CharacterCreationCampaignBehavior), nameof(CharacterCreationCampaignBehavior.InitializeData))]
    public class CharacterCreation : CharacterCreationCampaignBehavior, ICharacterCreationContentHandler
    {
        // these are excluded from the project and is intended for modders to use in their mod with losing countless hours everytime
        [HarmonyPrefix]
        static bool Prefix(ref CharacterCreation __instance, CharacterCreationManager characterCreationManager)
        {
            characterCreationManager.CharacterCreationContent.ChangeReviewPageDescription(new TextObject("{=W6pKpEoT}You prepare to set off for a grand adventure in Calradia! Here is your character. Continue if you are ready, or go back to make changes.", null));
            var ParentsMenu = new CharacterCreationParentsMenu();
            var ChildhoodMenu = new CharacterCreationChildhoodMenu();
            var EducationMenu = new CharacterCreationEducationMenu();
            var youthmenu = new CharacterCreationYouthMenu();
            var AdulthoodMenu = new CharacterCreationAdulthoodMenu();
            var AgeMenu = new CharacterCreationAgeMenu();
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