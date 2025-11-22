using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Anno_Domini_Calradia_1084.Patches
{
    [HarmonyPatch(typeof(TooltipRefresherCollection), "RefreshMobilePartyTooltip")]
    public static class PartyFoodTooltipPatch
    {
        // This runs after the vanilla tooltip fills its data
        [HarmonyPostfix]
        public static void AddFoodInfo(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
        {
            try
            {
                // Extract the party from args
                var mobileParty = args[0] as MobileParty;
                if (mobileParty == null) return;

                // Calculate total food quantity
                float totalFood = 0f;
                foreach (var element in mobileParty.ItemRoster)
                {
                    if (element.EquipmentElement.Item?.IsFood == true)
                        totalFood += element.Amount;
                }

                // Add under the "Information" section
                if (totalFood > 0)
                {
                    propertyBasedTooltipVM.AddProperty(
                        new TextObject("{=Food_Stock_Label}Food Stock", null).ToString(),
                        totalFood.ToString("0"),
                        0,
                        TooltipProperty.TooltipPropertyFlags.None
                    );
                }
                else
                {
                    propertyBasedTooltipVM.AddProperty(
                        new TextObject("{=Food_Stock_Label}Food Stock", null).ToString(),
                        new TextObject("{=Food_None}None").ToString(),
                        0,
                        TooltipProperty.TooltipPropertyFlags.None
                    );
                }
            }
            catch (System.Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Food tooltip patch error: {ex.Message}"));
            }
        }
    }
}
