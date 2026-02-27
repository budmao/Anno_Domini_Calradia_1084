/*

    ███████  █████  ██    ██ ███████     ██       ██████   █████  ██████      ███████  ██████ ██████  ███████ ███████ ███    ██ 
    ██      ██   ██ ██    ██ ██          ██      ██    ██ ██   ██ ██   ██     ██      ██      ██   ██ ██      ██      ████   ██ 
    ███████ ███████ ██    ██ █████       ██      ██    ██ ███████ ██   ██     ███████ ██      ██████  █████   █████   ██ ██  ██ 
         ██ ██   ██  ██  ██  ██          ██      ██    ██ ██   ██ ██   ██          ██ ██      ██   ██ ██      ██      ██  ██ ██ 
    ███████ ██   ██   ████   ███████     ███████  ██████  ██   ██ ██████      ███████  ██████ ██   ██ ███████ ███████ ██   ████ 
                                                                                                                            
    Fix: hero appears as shadow/silhouette on the save/load screen when mods are active.

    Vanilla code in SavedGameVM constructor blanks out MainHeroVisualCode and BannerTextCode
    whenever IsModuleDiscrepancyDetected is true (i.e. any mod was added/removed relative to
    the save). This causes SaveLoadMainHeroVisualWidget to fall back to the default shadow
    widget. The visual code itself is safe to display regardless of module state, so we restore
    it unconditionally from the save metadata.

    */

using System;
using HarmonyLib;
using SandBox.ViewModelCollection.SaveLoad;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

[HarmonyPatch(typeof(SavedGameVM))]
[HarmonyPatch(MethodType.Constructor, typeof(SaveGameFileInfo), typeof(bool), typeof(Action<SavedGameVM>), typeof(Action<SavedGameVM>), typeof(Action), typeof(Action), typeof(bool), typeof(bool))]
public static class SavedGameVM_Constructor_Patch
{
    public static void Postfix(SavedGameVM __instance, SaveGameFileInfo save)
    {
        try
        {
            // Vanilla clears these when any module discrepancy is detected (added/removed mods),
            // which makes the hero appear as a featureless shadow on the save/load screen.
            // The visual code is safe to show regardless of module state, so always restore it.
            if (string.IsNullOrEmpty(__instance.MainHeroVisualCode))
            {
                string visualCode = save.MetaData.GetCharacterVisualCode();
                if (!string.IsNullOrEmpty(visualCode))
                {
                    __instance.MainHeroVisualCode = visualCode;
                }
            }

            if (string.IsNullOrEmpty(__instance.BannerTextCode))
            {
                string bannerCode = save.MetaData.GetClanBannerCode();
                if (!string.IsNullOrEmpty(bannerCode))
                {
                    __instance.BannerTextCode = bannerCode;
                }
            }
        }
        catch (Exception ex)
        {
            //Main.DebugLog($"[SavedGameVM_Constructor_Patch] Failed to restore hero visual code: {ex.Message}");
        }
    }
}