using HarmonyLib;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using System.Linq;

namespace Anno_Domini_Calradia_1084.Patches
{

    [HarmonyPatch(typeof(ConversationManager), "FindMatchingTextOrNull")]
    public static class ConversationManager_FindMatchingTextOrNull_Patch
    {
        static void Prefix(CharacterObject character, out CultureObject __state)
        {
            // Save the original culture
            __state = character.Culture;

            // Temporarily assign the fake culture
            string fakeCulture = "empire";
            if (__state.StringId == "vlandia" || __state.StringId == "svadia")
            {
                fakeCulture = "vlandia";
            }
            else if (__state.StringId == "empire")
            {
                fakeCulture = "empire";
            }
            else if (__state.StringId == "aserai")
            {
                fakeCulture = "aserai";
            }
            else if (__state.StringId == "khuzait")
            {
                fakeCulture = "khuzait";
            }
            else if (__state.StringId == "battania")
            {
                fakeCulture = "battania";
            }
            else if (__state.StringId == "sturgia" || __state.StringId == "nord")
            {
                fakeCulture = "sturgia";
            }
            // aserai / khuzait

            character.Culture = Game.Current.ObjectManager.GetObjectTypeList<CultureObject>().FirstOrDefault(c => c.StringId == fakeCulture);
        }

        // Restore the original culture
        static void Postfix(CharacterObject character, CultureObject __state)
        {
            character.Culture = __state;
        }
    }
}