using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.GauntletUI.BaseTypes;

namespace Anno_Domini_Calradia_1084
{
    /// <summary>
    /// ULTIMATE WORKSHOP ICON PATCH - All 3 locations working perfectly!
    /// 
    /// Mappings:
    /// - bakery → mill (everywhere)
    /// - butcher → tannery (everywhere) 
    /// - stable → stable (pass-through, uses native stable icon)
    /// 
    /// CORRECTED: Town Management uses lowercase to match native brush style names
    /// </summary>
    public class WorkshopIcon_Patch
    {
        /// <summary>
        /// Clan/Manage Workshop mapping (lowercase, uses _large sprites)
        /// </summary>
        private static readonly Dictionary<string, string> ClanIconMap = new Dictionary<string, string>()
        {
            { "bakery", "mill" },
            { "butcher", "tannery" },
            { "stable", "stable" },
        };

        /// <summary>
        /// Town Management mapping (lowercase - matches native TownManagement brush style names)
        /// Note: We use lowercase because the RefreshValues patch converts ShopId to lowercase
        /// </summary>
        private static readonly Dictionary<string, string> TownIconMap = new Dictionary<string, string>()
        {
            { "bakery", "mill" },     
            { "butcher", "tannery" },  
            { "stable", "stable" },    
        };

        /// <summary>
        /// Patch 1: Clan list icons
        /// </summary>
        [HarmonyPatch(typeof(ClanFinanceWorkshopItemVM), "RefreshValues")]
        public class ClanFinanceWorkshopItemVM_RefreshValues
        {
            [HarmonyPostfix]
            public static void Postfix(ClanFinanceWorkshopItemVM __instance)
            {
                try
                {
                    Workshop workshop = GetWorkshop(__instance);
                    if (workshop?.WorkshopType == null) return;

                    string workshopId = workshop.WorkshopType.StringId?.ToLower();
                    if (string.IsNullOrEmpty(workshopId)) return;

                    if (ClanIconMap.TryGetValue(workshopId, out string replacementId))
                    {
                        PropertyInfo workshopTypeIdProp = __instance.GetType().GetProperty("WorkshopTypeId");
                        if (workshopTypeIdProp != null && workshopTypeIdProp.CanWrite)
                        {
                            workshopTypeIdProp.SetValue(__instance, replacementId);
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Patch 2: Town Management Constructor
        /// </summary>
        [HarmonyPatch(typeof(TownManagementShopItemVM), MethodType.Constructor, new Type[] { typeof(Workshop) })]
        public class TownManagementShopItemVM_Constructor
        {
            [HarmonyPostfix]
            public static void Postfix(TownManagementShopItemVM __instance, Workshop workshop)
            {
                try
                {
                    if (workshop?.WorkshopType == null) return;

                    string workshopId = workshop.WorkshopType.StringId?.ToLower();
                    if (string.IsNullOrEmpty(workshopId)) return;

                    if (TownIconMap.TryGetValue(workshopId, out string replacementId))
                    {
                        __instance.ShopId = replacementId;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Patch 3: Town Management RefreshValues
        /// </summary>
        [HarmonyPatch(typeof(TownManagementShopItemVM), "RefreshValues")]
        public class TownManagementShopItemVM_RefreshValues
        {
            [HarmonyPostfix]
            public static void Postfix(TownManagementShopItemVM __instance)
            {
                try
                {
                    string currentShopId = __instance.ShopId?.ToLower();
                    if (string.IsNullOrEmpty(currentShopId)) return;

                    // Now using lowercase dictionary, so this matches!
                    if (TownIconMap.TryGetValue(currentShopId, out string replacementId))
                    {
                        __instance.ShopId = replacementId;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Patch 4: BrushWidget.SetState - Handles Manage Workshop popup
        /// </summary>
        [HarmonyPatch(typeof(BrushWidget), "SetState")]
        public class BrushWidget_SetState_Workshop
        {
            [HarmonyPrefix]
            public static void Prefix(BrushWidget __instance, ref string stateName)
            {
                try
                {
                    if (__instance?.Brush == null) return;
                    if (string.IsNullOrEmpty(stateName)) return;

                    string brushName = __instance.Brush.Name;
                    if (string.IsNullOrEmpty(brushName)) return;

                    // Only intercept workshop-related brushes
                    if (brushName.Contains("Workshop") || brushName.Contains("Shop"))
                    {
                        string original = stateName.ToLower();

                        if (ClanIconMap.TryGetValue(original, out string replacement))
                        {
                            stateName = replacement;
                        }
                    }
                }
                catch { }
            }
        }

        private static Workshop GetWorkshop(ClanFinanceWorkshopItemVM instance)
        {
            try
            {
                PropertyInfo workshopProp = instance.GetType().GetProperty("Workshop");
                if (workshopProp != null)
                {
                    return workshopProp.GetValue(instance) as Workshop;
                }

                var fields = instance.GetType().GetFields(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(Workshop))
                    {
                        return field.GetValue(instance) as Workshop;
                    }
                }
            }
            catch { }
            return null;
        }
    }
}