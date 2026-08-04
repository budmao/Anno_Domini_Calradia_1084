using System;
using System.Collections.Generic;
using HarmonyLib;
using SandBox.Missions.MissionLogics;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.MountAndBlade;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(MissionAgentHandler), "GetAgentSettlementColors")]
    public static class TownsfolkColorPatch
    {
        private static readonly Random Rng = new Random();

        // ── Battania (Celtic — forest greens, mustards, earth browns) ──
        private static readonly uint[] BattaniaPrimary = new uint[]
        {
            0xFF284E19, 0xFFAEC382, 0xFF7F6B60, 0xFFCDA87C, 0xFF975B43,
            0xFF7B5E4E, 0xFF7E6E4A, 0xFF3A3321, 0xFF5D5B44, 0xFF726B3D,
            0xFFA97435, 0xFF34671E, 0xFF5F4F44
        };
        private static readonly uint[] BattaniaSecondary = new uint[]
        {
            0xFF284E19, 0xFFAEC382, 0xFF7F6B60, 0xFFCAC1BA, 0xFFCDA87C,
            0xFF975B43, 0xFF7B5E4E, 0xFF7E6E4A, 0xFF3A3321, 0xFF5D5B44,
            0xFF726B3D, 0xFFA97435, 0xFF5F4F44
        };

        // ── Aserai (Arabian — creams, sands, indigo, saffron) ──
        private static readonly uint[] AseraiPrimary = new uint[]
        {
            0xFF382188, 0xFFEFC990, 0xFF224277, 0xFFE7D3BA, 0xFFECE8DD,
            0xFFDCAC46, 0xFFE9E2C5, 0xFFEBDCBB, 0xFFE0C78E, 0xFFCDA87C,
            0xFFE6A57F, 0xFFB3A491
        };
        private static readonly uint[] AseraiSecondary = AseraiPrimary;

        // ── Vlandia (Norman — muted reds, grays, browns, some blue) ──
        private static readonly uint[] VlandiaPrimary = new uint[]
        {
            0xFF8D291A, 0xFF7F6B60, 0xFFB6ABA7, 0xFFCAC1BA, 0xFF975B43,
            0xFF7B5E4E, 0xFF3D2F22, 0xFF453E38, 0xFF830808, 0xFF2C4D86,
            0xFF8D5C44, 0xFFB3A491
        };
        private static readonly uint[] VlandiaSecondary = VlandiaPrimary;

        // ── Khuzait (Steppe — leather browns, dark earth tones) ──
        private static readonly uint[] KhuzaitPrimary = new uint[]
        {
            0xFFB57A1E, 0xFFCDA87C, 0xFF975B43, 0xFF7B5E4E, 0xFF714214,
            0xFF3A3321, 0xFF3D2F22, 0xFF453E38, 0xFFA97435, 0xFF41281B,
            0xFF8D5C44, 0xFF5F4F44
        };
        private static readonly uint[] KhuzaitSecondary = KhuzaitPrimary;

        // ── Sturgia (Slavic — grays, undyed wool, dark blues, browns) ──
        private static readonly uint[] SturgiaPrimary = new uint[]
        {
            0xFF224277, 0xFFC3C3C3, 0xFF7F6B60, 0xFFB6ABA7, 0xFFCAC1BA,
            0xFF3D2F22, 0xFF453E38, 0xFF515267, 0xFFCACCCB, 0xFF0B0C11,
            0xFF2C4D86, 0xFF5F4F44
        };
        private static readonly uint[] SturgiaSecondary = SturgiaPrimary;

        // ── Nord (Norse — deeper blues, grays, dark browns) ──
        private static readonly uint[] NordPrimary = new uint[]
        {
            0xFF224277, 0xFFC3C3C3, 0xFF7F6B60, 0xFFB6ABA7, 0xFFCAC1BA,
            0xFF3D2F22, 0xFF453E38, 0xFF515267, 0xFF0B0C11, 0xFF3A6298,
            0xFF2C4D86, 0xFF5F4F44
        };
        private static readonly uint[] NordSecondary = NordPrimary;

        // ── Empire (Byzantine — madder reds, ochres, mauves, warm tones) ──
        private static readonly uint[] EmpirePrimary = new uint[]
        {
            0xFFDEA940, 0xFF8D291A, 0xFF7F6B60, 0xFF967E7E, 0xFFCAC1BA,
            0xFFDCAC46, 0xFFCDA87C, 0xFFBD7E75, 0xFF975B43, 0xFFAC9188,
            0xFF830808, 0xFF6C1512, 0xFFB3A491
        };
        private static readonly uint[] EmpireSecondary = EmpirePrimary;

        // ── Fallback ──
        private static readonly uint[] FallbackPrimary = new uint[]
        {
            0xFF7F6B60, 0xFFB6ABA7, 0xFFCAC1BA, 0xFF3D2F22, 0xFF453E38,
            0xFFCDA87C, 0xFF975B43, 0xFF7B5E4E
        };
        private static readonly uint[] FallbackSecondary = FallbackPrimary;

        private static readonly Dictionary<string, (uint[] primary, uint[] secondary)> CulturePalettes =
            new Dictionary<string, (uint[], uint[])>
        {
            { "battania",  (BattaniaPrimary,  BattaniaSecondary)  },
            { "aserai",    (AseraiPrimary,    AseraiSecondary)    },
            { "vlandia",   (VlandiaPrimary,   VlandiaSecondary)   },
            { "khuzait",   (KhuzaitPrimary,   KhuzaitSecondary)  },
            { "sturgia",   (SturgiaPrimary,   SturgiaSecondary)   },
            { "nord",      (NordPrimary,      NordSecondary)      },
            { "empire",    (EmpirePrimary,    EmpireSecondary)    },
        };

        static void Postfix(LocationCharacter locationCharacter, ref ValueTuple<uint, uint> __result)
        {
            if (locationCharacter.Character.IsHero)
                return;

            if (locationCharacter.Character.IsSoldier)
                return;

            // Skip prison guards — keep faction colors
            if (locationCharacter.Character.StringId != null &&
                locationCharacter.Character.StringId.Contains("prison_guard"))
                return;

            if (Mission.Current?.GetMissionBehavior<TournamentBehavior>() != null)
                return;

            string cultureId = Settlement.CurrentSettlement?.Culture?.StringId;

            uint[] primaryPool;
            uint[] secondaryPool;

            if (cultureId != null && CulturePalettes.TryGetValue(cultureId, out var palette))
            {
                primaryPool = palette.primary;
                secondaryPool = palette.secondary;
            }
            else
            {
                primaryPool = FallbackPrimary;
                secondaryPool = FallbackSecondary;
            }

            uint primary = primaryPool[Rng.Next(primaryPool.Length)];
            uint secondary = secondaryPool[Rng.Next(secondaryPool.Length)];
            __result = new ValueTuple<uint, uint>(primary, secondary);

            //Main.DebugLog($"TownsfolkColor: {locationCharacter.Character?.Name} culture={cultureId ?? "unknown"} → Primary={primary:X8}, Secondary={secondary:X8}");
        }
    }
}