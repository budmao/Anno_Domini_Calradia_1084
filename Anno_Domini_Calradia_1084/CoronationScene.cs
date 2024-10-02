using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Anno_Domini_Calradia_1084
{
    internal class CoronationScene
    {
        [HarmonyPatch(typeof(CampaignSceneNotificationHelper), "GetBodyguardOfCulture")]
        public class CampaignSceneNotificationHelper_GetBodyguardOfCulture_Patch
        {
            private static bool Prefix(ref SceneNotificationData.SceneNotificationCharacter __result, CultureObject culture)
            {
                string stringId = culture.StringId;
                string troopId = "AOM_khuzait_irregular_rt2";
                if (stringId == "nord")
                {
                    troopId = "AOM_nord_regular_xt5";
                }
                else if (stringId == "svadia")
                {
                    troopId = "AOM_svadia_regular_st5";
                }
                else if (stringId == "sturgia")
                {
                    troopId = "AOM_sturgia_regular_xt5";
                }
                else if (stringId == "vlandia")
                {
                    troopId = "AOM_vlandia_regular_st5";
                }
                else if (stringId == "battania")
                {
                    troopId = "AOM_battania_regular_xt5";
                }
                else if (stringId == "khuzait")
                {
                    troopId = "AOM_khuzait_regular_xt5";
                }
                else if (stringId == "aserai")
                {
                    troopId = "AOM_aserai_regular_st5";
                }
                else if (stringId == "empire")
                {
                    troopId = "AOM_empire_regular_st5";
                }
                __result = new SceneNotificationData.SceneNotificationCharacter(MBObjectManager.Instance.GetObject<CharacterObject>(troopId), null, default(BodyProperties), false, uint.MaxValue, uint.MaxValue, false);
                return false;
            }
        }
    }
}
