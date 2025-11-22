using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using TaleWorlds.Core;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(DefaultItemCategories), MethodType.Constructor)]
    public class DefaultItemCategories_Patch
    {
        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount and Blade II Bannerlord",
            "Configs",
            "Anno_Domini_Calradia_1084_Debug.log"
        );

        [HarmonyPostfix]
        public static void Postfix(DefaultItemCategories __instance)
        {
            try
            {
                FieldInfo sheepField = typeof(DefaultItemCategories).GetField("_itemCategorySheep", BindingFlags.NonPublic | BindingFlags.Instance);
                ItemCategory sheepCategory = sheepField != null ? (ItemCategory)sheepField.GetValue(__instance) : null;
                FieldInfo grainField = typeof(DefaultItemCategories).GetField("_itemCategoryGrain", BindingFlags.NonPublic | BindingFlags.Instance);
                ItemCategory grainCategory = grainField != null ? (ItemCategory)grainField.GetValue(__instance) : null;
                FieldInfo grapeField = typeof(DefaultItemCategories).GetField("_itemCategoryGrape", BindingFlags.NonPublic | BindingFlags.Instance);
                ItemCategory grapeCategory = grapeField != null ? (ItemCategory)grapeField.GetValue(__instance) : null;



                // Helper method to register categories
                Action<string, int, int, ItemCategory, bool, ItemCategory.Property> RegisterCategory
                    = (id, baseDemand, luxuryDemand, substitute, isAnimal, property) =>
                    {
                        ItemCategory.Property props = property;

                        ItemCategory category = Game.Current.ObjectManager.RegisterPresumedObject<ItemCategory>(new ItemCategory(id));
                    category.InitializeObject(
                        isTradeGood: true,
                        baseDemand: baseDemand,
                        luxuryDemand: luxuryDemand,
                        properties: props,
                        canSubstitute: substitute,
                        substitutionFactor: substitute != null ? 0.1f : 0f,
                        isAnimal: isAnimal,
                        isValid: true
                    );

                    // Build capitalized name for reflection
                    string capName;
                    if (!string.IsNullOrEmpty(id))
                    {
                        if (id.Length == 1)
                            capName = char.ToUpper(id[0]).ToString();
                        else
                            capName = char.ToUpper(id[0]) + id.Substring(1);
                    }
                    else
                    {
                        capName = id;
                    }

                    // Try to set private field _itemCategoryXxx
                    var field = typeof(DefaultItemCategories).GetField("_itemCategory" + capName, BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                        field.SetValue(__instance, category);

                    // Try to set public property Xxx
                    var prop = typeof(DefaultItemCategories).GetProperty(capName, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null)
                        prop.SetValue(__instance, category);

                        // Log each category creation
                        File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Registered ItemCategory: {id}\n");

                    };

                // --- Livestock ---
                RegisterCategory("goose", 8, 0, sheepCategory, true, ItemCategory.Property.None);       // basic animal like sheep
                RegisterCategory("chicken", 8, 0, sheepCategory, true, ItemCategory.Property.None);
                RegisterCategory("hare", 4, 8, sheepCategory, true, ItemCategory.Property.None); // rare, adds wealth

                // --- Crustaceans ---
                RegisterCategory("crab", 10, 15, null, true, ItemCategory.Property.BonusToFoodStores);  // food supply
                RegisterCategory("lobster", 5, 50, null, true, ItemCategory.Property.BonusToLoyalty);   // luxury
                RegisterCategory("lobster_cooked", 3, 100, null, false, ItemCategory.Property.BonusToProsperity); // luxury delicacy

                // --- Processed / Luxury Meats ---
                RegisterCategory("meat_sausage", 4, 80, null, false, ItemCategory.Property.BonusToLoyalty); // salted/processed, adds wealth
                RegisterCategory("meat_ham", 4, 80, null, false, ItemCategory.Property.BonusToLoyalty);       // rare luxury ham
                RegisterCategory("meat_poultry_cooked", 4, 80, null, false, ItemCategory.Property.BonusToLoyalty);
                RegisterCategory("meat_mutton_cooked", 4, 80, null, false, ItemCategory.Property.BonusToLoyalty);
                RegisterCategory("meat_pork_cooked", 4, 80, null, false, ItemCategory.Property.BonusToLoyalty);

                // --- Staple Meats ---
                RegisterCategory("meat_beef", 20, 40, null, false, ItemCategory.Property.BonusToFoodStores);       // widely consumed
                RegisterCategory("meat_poultry", 15, 25, null, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("meat_mutton", 18, 30, null, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("meat_pork", 17, 30, null, false, ItemCategory.Property.BonusToFoodStores);

                // --- Wild Game ---
                RegisterCategory("meat_hare", 5, 15, null, false, ItemCategory.Property.BonusToLoyalty);       // occasional luxury

                // --- Fish ---
                RegisterCategory("small_fish", 15, 15, grainCategory, false, ItemCategory.Property.BonusToFoodStores); // same as vanilla fish

                // --- Vegetables ---
                RegisterCategory("cabbage_vegetable", 10, 2, grainCategory, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("leek_vegetable", 10, 2, grainCategory, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("carrot_vegetable", 10, 2, grainCategory, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("mushroom_vegetable", 8, 4, grainCategory, false, ItemCategory.Property.BonusToFoodStores);

                // --- Fruits ---
                RegisterCategory("apple_fruit", 10, 5, grainCategory, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("orange_fruit", 7, 10, grapeCategory, false, ItemCategory.Property.BonusToFoodStores); // exotic
                RegisterCategory("melon_fruit", 5, 20, grapeCategory, false, ItemCategory.Property.BonusToLoyalty);  // luxury fruit

                // --- Other ---
                RegisterCategory("honey", 10, 20, grainCategory, false, ItemCategory.Property.BonusToProsperity); // valuable trade food
                RegisterCategory("milk", 10, 0, grainCategory, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("heavy_cream", 10, 10, grainCategory, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("bread", 80, 0, grainCategory, false, ItemCategory.Property.BonusToFoodStores);   // staple
                RegisterCategory("pie", 8, 25, grainCategory, false, ItemCategory.Property.BonusToProsperity);   // processed food, minor luxury
                RegisterCategory("dough", 15, 5, grainCategory, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("flour", 15, 5, grainCategory, false, ItemCategory.Property.BonusToFoodStores);
                RegisterCategory("nuts", 8, 15, grainCategory, false, ItemCategory.Property.BonusToProsperity);  // semi-luxury snack
                RegisterCategory("stew", 12, 12, grainCategory, false, ItemCategory.Property.BonusToProsperity);

                // --- Alcohol ---
                RegisterCategory("mead", 15, 30, null, false, ItemCategory.Property.BonusToLoyalty);
                RegisterCategory("ale", 23, 20, null, false, ItemCategory.Property.BonusToFoodStores);   // staple beverage
                RegisterCategory("cider", 10, 15, null, false, ItemCategory.Property.BonusToLoyalty);

                // --- Tradegoods ---
                RegisterCategory("small_hides", 30, 15, null, false, ItemCategory.Property.BonusToProsperity);
                RegisterCategory("medium_hides", 30, 15, null, false, ItemCategory.Property.BonusToProsperity);
                RegisterCategory("black_pepper", 10, 50, null, false, ItemCategory.Property.BonusToProsperity);


                // --- Done ---
                File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Custom ItemCategories successfully registered.\n");

            }
            catch (Exception ex)
            {
                File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR in DefaultItemCategories patch: {ex}\n");

            }
        }
    }
}
