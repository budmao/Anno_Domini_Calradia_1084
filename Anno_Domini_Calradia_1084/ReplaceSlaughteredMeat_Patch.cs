using HarmonyLib;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;
using System.Reflection;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

[HarmonyPatch(typeof(InventoryLogic), "SlaughterItem")]
public class ReplaceSlaughteredMeatPatch
{
    static bool Prefix(InventoryLogic __instance, ItemRosterElement itemRosterElement)
    {
        var item = itemRosterElement.EquipmentElement.Item;

        // Only affect livestock types we want to handle
        if (item.StringId == "sheep" || item.StringId == "hog" || item.StringId == "cow"
            || item.StringId == "chicken_product" || item.StringId == "goose_product" || item.StringId == "hare")
        {
            int meatCount = item.HorseComponent?.MeatCount ?? 1;
            int hideCount = item.HorseComponent?.HideCount ?? 1;

            // Override specific animal yields
            switch (item.StringId)
            {
                case "cow":
                    meatCount = 4;
                    hideCount = 2;
                    break;
                case "chicken_product":
                case "goose_product":
                    meatCount = 1;
                    hideCount = 0;
                    break;
                case "hare":
                    meatCount = 1;
                    hideCount = 1;
                    break;
            }

            // Access private _rosters field
            var rostersField = AccessTools.Field(typeof(InventoryLogic), "_rosters");
            var rosters = (ItemRoster[])rostersField.GetValue(__instance);
            ItemRoster playerInventory = rosters[1];

            List<TransferCommandResult> transferList = new List<TransferCommandResult>();

            // Remove one animal
            bool isSingle = itemRosterElement.Amount == 1;
            int index = playerInventory.AddToCounts(itemRosterElement.EquipmentElement, -1);
            if (isSingle)
            {
                transferList.Add(new TransferCommandResult(
                    InventoryLogic.InventorySide.PlayerInventory,
                    new ItemRosterElement(itemRosterElement.EquipmentElement, 0),
                    -1, 0, EquipmentIndex.None, null,
                    itemRosterElement.EquipmentElement.Item.IsCivilian));
            }
            else
            {
                var elementCopy = playerInventory.GetElementCopyAtIndex(index);
                transferList.Add(new TransferCommandResult(
                    InventoryLogic.InventorySide.PlayerInventory,
                    elementCopy, -1, elementCopy.Amount, EquipmentIndex.None, null,
                    elementCopy.EquipmentElement.Item.IsCivilian));
            }

            // Determine correct meat type
            ItemObject meatItem = null;
            switch (item.StringId)
            {
                case "sheep":
                    meatItem = Game.Current.ObjectManager.GetObject<ItemObject>("meat_mutton");
                    break;
                case "hog":
                    meatItem = Game.Current.ObjectManager.GetObject<ItemObject>("meat_pork");
                    break;
                case "cow":
                    meatItem = Game.Current.ObjectManager.GetObject<ItemObject>("meat_beef");
                    break;
                case "chicken_product":
                case "goose_product":
                    meatItem = Game.Current.ObjectManager.GetObject<ItemObject>("meat_poultry");
                    break;
                case "hare":
                    meatItem = Game.Current.ObjectManager.GetObject<ItemObject>("meat_hare");
                    break;
            }

            if (meatItem == null)
                meatItem = DefaultItems.Meat;

            // Add meat
            int meatIndex = playerInventory.AddToCounts(meatItem, meatCount);
            var meatElement = playerInventory.GetElementCopyAtIndex(meatIndex);
            transferList.Add(new TransferCommandResult(
                InventoryLogic.InventorySide.PlayerInventory,
                meatElement, meatCount, meatElement.Amount,
                EquipmentIndex.None, null, meatElement.EquipmentElement.Item.IsCivilian));

            // Determine correct hide type
            ItemObject hideItem = null;
            switch (item.StringId)
            {
                case "sheep":
                    hideItem = Game.Current.ObjectManager.GetObject<ItemObject>("medium_hides");
                    break;
                case "hog":
                    hideItem = Game.Current.ObjectManager.GetObject<ItemObject>("medium_hides");
                    break;
                case "cow":
                    hideItem = Game.Current.ObjectManager.GetObject<ItemObject>("hides");
                    break;
                case "hare":
                    hideItem = Game.Current.ObjectManager.GetObject<ItemObject>("small_hides");
                    break;
                case "chicken_product":
                case "goose_product":
                    hideItem = null; // Poultry has no usable hides
                    break;
            }

            // Add hides (if any)
            if (hideCount > 0 && hideItem != null)
            {
                int hideIndex = playerInventory.AddToCounts(hideItem, hideCount);
                var hideElement = playerInventory.GetElementCopyAtIndex(hideIndex);
                transferList.Add(new TransferCommandResult(
                    InventoryLogic.InventorySide.PlayerInventory,
                    hideElement, hideCount, hideElement.Amount,
                    EquipmentIndex.None, null, hideElement.EquipmentElement.Item.IsCivilian));
            }


            // Update UI and state
            MethodInfo setStateMethod = AccessTools.Method(typeof(InventoryLogic), "SetCurrentStateAsInitial");
            MethodInfo onAfterTransferMethod = AccessTools.Method(typeof(InventoryLogic), "OnAfterTransfer");

            setStateMethod.Invoke(__instance, null);
            onAfterTransferMethod.Invoke(__instance, new object[] { transferList });

            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(FoodConsumptionBehavior), "SlaughterLivestock")]
public class ReplaceAutoSlaughteredMeatPatch
{
    static bool Prefix(MobileParty party, int partyRemainingFoodPercentage, ref bool __result)
    {
        ItemRoster itemRoster = party.ItemRoster;
        bool slaughteredAny = false;

        for (int i = itemRoster.Count - 1; i >= 0; i--)
        {
            ItemObject item = itemRoster.GetItemAtIndex(i);
            HorseComponent horse = item.HorseComponent;

            if (horse != null && horse.IsLiveStock)
            {
                while (partyRemainingFoodPercentage < 0 && itemRoster.GetElementCopyAtIndex(i).Amount > 0)
                {
                    itemRoster.AddToCounts(item, -1);
                    slaughteredAny = true;

                    // Base from horse component
                    int meatCount = horse.MeatCount;
                    int hideCount = horse.HideCount;
                    ItemObject meat = null;

                    switch (item.StringId)
                    {
                        case "sheep":
                            meat = Game.Current.ObjectManager.GetObject<ItemObject>("meat_mutton");
                            break;
                        case "hog":
                            meat = Game.Current.ObjectManager.GetObject<ItemObject>("meat_pork");
                            break;
                        case "cow":
                            meat = Game.Current.ObjectManager.GetObject<ItemObject>("meat_beef");
                            meatCount = 4;
                            hideCount = 2;
                            break;
                        case "chicken_product":
                        case "goose_product":
                            meat = Game.Current.ObjectManager.GetObject<ItemObject>("meat_poultry");
                            meatCount = 1;
                            hideCount = 0;
                            break;
                        case "hare":
                            meat = Game.Current.ObjectManager.GetObject<ItemObject>("meat_hare");
                            meatCount = 1;
                            hideCount = 1;
                            break;
                        default:
                            meat = DefaultItems.Meat;
                            break;
                    }


                    if (meat != null)
                        itemRoster.AddToCounts(meat, meatCount);

                    ItemObject hide = null;
                    switch (item.StringId)
                    {
                        case "sheep":
                            hide = Game.Current.ObjectManager.GetObject<ItemObject>("medium_hides");
                            break;
                        case "hog":
                            hide = Game.Current.ObjectManager.GetObject<ItemObject>("medium_hides");
                            break;
                        case "cow":
                            hide = Game.Current.ObjectManager.GetObject<ItemObject>("hides");
                            break;
                        case "hare":
                            hide = Game.Current.ObjectManager.GetObject<ItemObject>("small_hides");
                            break;
                        case "chicken_product":
                        case "goose_product":
                            hide = null; // no hide for poultry
                            break;
                    }

                    if (hideCount > 0 && hide != null)
                        itemRoster.AddToCounts(hide, hideCount);


                    partyRemainingFoodPercentage += meatCount * 100;
                }
            }
        }

        if (slaughteredAny)
        {
            __result = true;
            return false;
        }

        return true;
    }
}
