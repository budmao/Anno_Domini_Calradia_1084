using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Anno_Domini_Calradia_1084
{
    // Token: 0x02000028 RID: 40
    [HarmonyPatch(typeof(SandboxCharacterCreationContent))]
    [HarmonyPatch("OnCharacterCreationFinalized")]
    public class CustomStartingPositions_Patch
    {
        // Token: 0x06000161 RID: 353 RVA: 0x0000D2B4 File Offset: 0x0000B4B4
        public static bool Prefix()
        {
            CultureObject culture = CharacterObject.PlayerCharacter.Culture;
            bool flag3 = culture.StringId == "nord";
            if (flag3)
            {
                MobileParty.MainParty.Position2D = new Vec2(349.5959f, 547.5106f);
            }
            else
            {
                bool flag4 = culture.StringId == "svadia";
                if (flag4)
                {
                    MobileParty.MainParty.Position2D = new Vec2(374.0022f, 365.0201f);
                }
                else
                {
                    bool flag5 = culture.StringId == "sturgia";
                    if (flag5)
                    {
                        MobileParty.MainParty.Position2D = new Vec2(532.5359f, 560.1585f);
                    }
                    else
                    {
                        bool flag6 = culture.StringId == "vlandia";
                        if (flag6)
                        {
                            MobileParty.MainParty.Position2D = new Vec2(117.2503f, 399.7682f);
                        }
                        else
                        {
                            bool flag7 = culture.StringId == "battania";
                            if (flag7)
                            {
                                MobileParty.MainParty.Position2D = new Vec2(268.8808f, 497.6864f);
                            }
                            else
                            {
                                bool flag8 = culture.StringId == "khuzait";
                                if (flag8)
                                {
                                    MobileParty.MainParty.Position2D = new Vec2(733.1702f, 430.6699f);
                                }
                                else
                                {
                                    bool flag9 = culture.StringId == "aserai";
                                    if (flag9)
                                    {
                                        MobileParty.MainParty.Position2D = new Vec2(389.1162f, 106.3882f);
                                    }
                                    else
                                    {
                                        MobileParty.MainParty.Position2D = new Vec2(562.7066f, 330.8654f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            MapState mapState;
            bool flag2 = (mapState = (GameStateManager.Current.ActiveState as MapState)) != null;
            bool flag10 = flag2;
            if (flag10)
            {
                mapState.Handler.ResetCamera(true, true);
                mapState.Handler.TeleportCameraToMainParty();
            }
            return false;
        }
    }
}
