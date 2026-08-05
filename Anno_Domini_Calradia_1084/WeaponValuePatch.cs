using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Anno_Domini_Calradia_1084
{
    [HarmonyPatch(typeof(DefaultItemValueModel), nameof(DefaultItemValueModel.CalculateValue))]
    public static class WeaponValuePatch
    {
        private const float SWORD_MULTIPLIER = 3.0f;
        private const float DAGGER_MULTIPLIER = 1.5f;
        private const float SPEAR_MULTIPLIER = 1.5f;
        private const float AXE_MULTIPLIER = 2.0f;
        private const float MACE_MULTIPLIER = 2.0f;

        [HarmonyPostfix]
        public static void Postfix(ref int __result, ItemObject item)
        {
            if (item?.PrimaryWeapon == null)
                return;

            float multiplier = GetMultiplier(item.PrimaryWeapon.WeaponClass);
            if (multiplier != 1f)
            {
                __result = MathF.Max(1, MathF.Round(__result * multiplier));
            }
        }

        private static float GetMultiplier(WeaponClass wc)
        {
            switch (wc)
            {
                case WeaponClass.OneHandedSword:
                case WeaponClass.TwoHandedSword:
                    return SWORD_MULTIPLIER;

                case WeaponClass.Dagger:
                    return DAGGER_MULTIPLIER;

                case WeaponClass.OneHandedPolearm:
                case WeaponClass.TwoHandedPolearm:
                case WeaponClass.LowGripPolearm:
                    return SPEAR_MULTIPLIER;

                case WeaponClass.OneHandedAxe:
                case WeaponClass.TwoHandedAxe:
                    return AXE_MULTIPLIER;

                case WeaponClass.Mace:
                case WeaponClass.TwoHandedMace:
                    return MACE_MULTIPLIER;

                default:
                    return 1f;
            }
        }
    }
}