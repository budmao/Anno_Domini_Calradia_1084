using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(SandboxCharacterCreationContent))]
    [HarmonyPatch("OnCharacterCreationFinalized")]
    public class CustomStartingPositions_Patch
    {
        public static bool Prefix()
        {
            CultureObject culture = CharacterObject.PlayerCharacter.Culture;

            if (culture.StringId == "nord")
            {
                MobileParty.MainParty.Position2D = new Vec2(444.4771f, 516.6031f);
            }
            else if (culture.StringId == "svadia")
            {
                MobileParty.MainParty.Position2D = new Vec2(379.0639f, 347.452f);
            }
            else if (culture.StringId == "sturgia")
            {
                MobileParty.MainParty.Position2D = new Vec2(532.5359f, 560.1585f);
            }
            else if (culture.StringId == "vlandia")
            {
                MobileParty.MainParty.Position2D = new Vec2(117.2503f, 399.7682f);
            }
            else if (culture.StringId == "balion")
            {
                MobileParty.MainParty.Position2D = new Vec2(180.4656f, 526.4017f);
            }
            else if (culture.StringId == "battania")
            {
                MobileParty.MainParty.Position2D = new Vec2(268.8808f, 497.6864f);
            }
            else if (culture.StringId == "khuzait")
            {
                MobileParty.MainParty.Position2D = new Vec2(733.1702f, 430.6699f);
            }
            else if (culture.StringId == "aserai")
            {
                MobileParty.MainParty.Position2D = new Vec2(389.1162f, 106.3882f);
            }
            else
            {
                MobileParty.MainParty.Position2D = new Vec2(562.7066f, 330.8654f);
            }

            if (GameStateManager.Current.ActiveState is MapState mapState)
            {
                mapState.Handler.ResetCamera(true, true);
                mapState.Handler.TeleportCameraToMainParty();
            }

            return false;
        }
    }
}
