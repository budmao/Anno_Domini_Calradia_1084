using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace Anno_Domini_Calradia_1084.Patches
{
    public class BanditShieldColorMissionBehavior : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        private readonly Dictionary<Clan, (uint primary, uint icon)> _savedBannerColors =
            new Dictionary<Clan, (uint, uint)>();

        // Only clans that should also get shield recoloring
        private static readonly Dictionary<string, (uint, uint)> ShieldOverrides =
            new Dictionary<string, (uint, uint)>
        {
            { "mountain_bandits", (0xFF830808, 0xFF2C4D86) },
        };

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();

            foreach (var kvp in ShieldOverrides)
            {
                Clan clan = Clan.All.FirstOrDefault(c => c.StringId == kvp.Key);
                if (clan?.Banner == null) continue;

                _savedBannerColors[clan] = (
                    clan.Banner.GetPrimaryColor(),
                    clan.Banner.GetFirstIconColor()
                );

                clan.Banner.ChangePrimaryColor(kvp.Value.Item1);
                clan.Banner.ChangeIconColors(kvp.Value.Item2);

                //Main.DebugLog($"BanditShieldColor: Swapped banner for {clan.StringId} during mission");
            }
        }

        protected override void OnEndMission()
        {
            base.OnEndMission();

            foreach (var kvp in _savedBannerColors)
            {
                kvp.Key.Banner?.ChangePrimaryColor(kvp.Value.primary);
                kvp.Key.Banner?.ChangeIconColors(kvp.Value.icon);

                //Main.DebugLog($"BanditShieldColor: Restored banner for {kvp.Key.StringId}");
            }

            _savedBannerColors.Clear();
        }
    }
}