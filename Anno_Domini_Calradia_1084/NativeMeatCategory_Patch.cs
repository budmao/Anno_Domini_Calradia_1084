using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using System.Reflection;

namespace Anno_Domini_Calradia_1084.Patches
{
    /// <summary>
    /// Patch to change the native meat category demand values to 0
    /// This will make vanilla "meat" have no base or luxury demand
    /// </summary>
    [HarmonyPatch(typeof(DefaultItemCategories), "InitializeAll")]
    public class NativeMeatCategory_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(DefaultItemCategories __instance)
        {
            // Get the private _itemCategoryMeat field
            var meatCategoryField = typeof(DefaultItemCategories).GetField(
                "_itemCategoryMeat",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (meatCategoryField != null)
            {
                ItemCategory meatCategory = (ItemCategory)meatCategoryField.GetValue(__instance);

                if (meatCategory != null)
                {
                    // Re-initialize the meat category with 0 demand and 0 luxury demand
                    // Also remove the substitution ability since it won't be traded
                    meatCategory.InitializeObject(
                        isTradeGood: true,
                        baseDemand: 0,              // Changed from 19 to 0
                        luxuryDemand: 0,            // Changed from 50 to 0
                        properties: ItemCategory.Property.BonusToFoodStores,
                        canSubstitute: null,        // Changed from fish to null (no substitution)
                        substitutionFactor: 0f,
                        isAnimal: false,
                        isValid: true
                    );
                }
            }
        }
    }

    /// <summary>
    /// Patch to reduce the native meat item value by 50%, rename it, and change its mesh
    /// Changes value from 30 to 15
    /// Changes name to "Mystery Meat" to distinguish from specific meat types
    /// Changes mesh to 'cooked_mutton_rotten' to make it visually distinct
    /// </summary>
    [HarmonyPatch(typeof(DefaultItems), "InitializeAll")]
    public class NativeMeatItem_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(DefaultItems __instance)
        {
            // Get the private _itemMeat field directly (can't use static property during initialization)
            var meatItemField = typeof(DefaultItems).GetField(
                "_itemMeat",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (meatItemField != null)
            {
                ItemObject meatItem = (ItemObject)meatItemField.GetValue(__instance);

                if (meatItem != null)
                {
                    // Change the name to "Mystery Meat"
                    var nameProperty = typeof(ItemObject).GetProperty("Name");
                    if (nameProperty != null)
                    {
                        var newName = new TextObject("{=ADC_mysteryMeat}Mystery Meat{@Plural}loads of mystery meat{\\@}", null);
                        nameProperty.SetValue(meatItem, newName);
                    }

                    // Change the mesh to 'cooked_mutton_rotten'
                    var meshProperty = typeof(ItemObject).GetProperty("MultiMeshName");
                    if (meshProperty != null)
                    {
                        meshProperty.SetValue(meatItem, "cooked_mutton_rotten");
                    }

                    // Use reflection to set the Value property (it's read-only with private setter)
                    var valueProperty = typeof(ItemObject).GetProperty("Value");
                    if (valueProperty != null)
                    {
                        // Change value from 30 to 15 (50% reduction)
                        valueProperty.SetValue(meatItem, 1);
                    }
                }
            }
        }
    }
}