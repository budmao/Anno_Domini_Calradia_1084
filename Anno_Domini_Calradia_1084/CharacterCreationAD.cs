using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Localization;

namespace Anno_Domini_Calradia_1084
{
    [HarmonyPatch(typeof(CharacterCreationCampaignBehavior), nameof(CharacterCreationCampaignBehavior.InitializeData))]
    public class CharacterCreationAD : CharacterCreationCampaignBehavior, ICharacterCreationContentHandler
    {
        // these are excluded from the project and is intended for modders to use in their mod with losing countless hours everytime
        [HarmonyPrefix]
        static bool Prefix(ref CharacterCreationAD __instance, CharacterCreationManager characterCreationManager)
        {
            Main.Log("CharacterCreation patch fired — using custom menus.");

            characterCreationManager.CharacterCreationContent.ChangeReviewPageDescription(new TextObject("{=W6pKpEoT}You prepare to set off for a grand adventure in Calradia! Here is your character. Continue if you are ready, or go back to make changes.", null));
            var ParentsMenu = new CharacterCreationParentsMenuAD();
            var ChildhoodMenu = new CharacterCreationChildhoodMenuAD();
            var EducationMenu = new CharacterCreationEducationMenuAD();
            var youthmenu = new CharacterCreationYouthMenuAD();
            var AdulthoodMenu = new CharacterCreationAdulthoodMenuAD();
            var AgeMenu = new CharacterCreationAgeMenuAD();
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