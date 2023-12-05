using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Array = System.Array;

public enum StorageSlotType
{
    Uninitialized = 0,
    PlayerInventory = 10,
    Hotbar = 20,
    TownStorage = 30,
}

public static class StorageController
{
    public const string INVENTORY_SLOT_RESPATH = "res://scenes/ui/inventory/inventory_slot.tscn";
    public const string HOTBAR_SLOT_RESPATH = "res://scenes/ui/inventory/hotbar_slot.tscn";
    public const string TOWN_STORAGE_SLOT_RESPATH = "res://scenes/ui/inventory/town_storage_slot.tscn";


    ///  <summary> Main Storage additive operation </summary>
    ///  <param name="storageGridContainer"></param>
    ///  <param name="rawArrayToAddTo"> The array of RawInventoryItem to add the item to</param>
    ///  <param name="rawItem"> Includes id, name and quantity to add </param>
    ///  <param name="index"> optional: index in the inventory array </param>
    ///  <param name="affectSelectedItem"> optional : affect the current selected item </param>
    ///  <param name="deselectOnAllAdded"> optional : deselect current item if all items were added </param>
    ///  
    /// 	<returns> how many items could *NOT* be added </returns>
    public static async Task<int> AddItem(GridContainer storageGridContainer, Array<RawInventoryItem> rawArrayToAddTo,
        RawInventoryItem rawItem, int index = -1)
    {
        if (rawItem == null)
        {
            GD.PrintErr("Tried to add a null item to inventory @StorageController.AddItem!");
            return -1;
        }

        rawItem.quantity = index == -1
            ? await AddToGridUntilFull(storageGridContainer, rawArrayToAddTo, rawItem) // index not specified
            : await AddToSlotUntilFull(storageGridContainer, rawArrayToAddTo, rawItem, index); // index given


        await SaveData.SyncInventory();
        return rawItem.quantity;
    }

    static async Task<int> AddToGridUntilFull(GridContainer storageGridContainer, Array<RawInventoryItem> rawArray,
        RawInventoryItem itemToAdd)
    {
        int quantity = itemToAdd.quantity;

        for (int i = 0; i < rawArray.Count; i++)
        {
            RawInventoryItem itemInSlot = rawArray[i];

            if (itemInSlot == null || itemToAdd.id == itemInSlot.id)
            {
                quantity = await AddToSlotUntilFull(storageGridContainer, rawArray, itemToAdd, i);
            }

            if (quantity == 0) break;
        }

        return quantity;
    }

    static Task<int> AddToSlotUntilFull(GridContainer storageGridContainer, Array<RawInventoryItem> rawArray,
        RawInventoryItem itemToAdd, int index)
    {
        if (index >= rawArray.Count) return Task.FromResult(itemToAdd.quantity);

        RawInventoryItem itemInSlot = rawArray[index];


        int spaceRemainingAtIndex = itemInSlot?.SpaceRemainingInStack ?? itemToAdd.stackSize;

        int howManyWereAdded = spaceRemainingAtIndex - itemToAdd.quantity < 0
            ? spaceRemainingAtIndex
            : itemToAdd.quantity;

        if (howManyWereAdded > 0)
        {
            RawInventoryItem addedItem = new RawInventoryItem(
                itemToAdd.id,
                itemToAdd.name,
                (itemInSlot?.quantity ?? 0) + howManyWereAdded,
                itemToAdd.stackSize);

            UpdateStorageSlot(storageGridContainer, rawArray, addedItem, index);

            itemToAdd.quantity -= howManyWereAdded;
        }

        HandleRemainderOfAddOrRemoveOperation(itemToAdd);

        return Task.FromResult(itemToAdd.quantity);
    }

    ///  <summary> storage item removal method </summary>
    ///  <param name="rawArray"></param>
    ///  <param name="rawItem"> Includes id, name and quantity to remove. </param>
    ///  <param name="index"> (Optional) If specified, only removes the quantity available in that index. </param>
    ///  <param name="storageGridContainer"></param>
    ///  <returns> Whether the removal operation was successful or not </returns>
    public static async Task<bool> RemoveItemFromStorage(
        GridContainer storageGridContainer, Array<RawInventoryItem> rawArray, RawInventoryItem rawItem, int index = -1)
    {
        if (rawItem == null)
        {
            GD.PushError("Tried to remove null item from inventory");
            return false;
        }

        if (!StorageData.ExistsInStorage(rawArray, rawItem.id, rawItem.quantity))
        {
            return false;
        }

        if (index >= 0)
        {
            return await RemoveFromSlotUntilEmpty(
                storageGridContainer, rawArray, rawItem.quantity, index, true) == 0;
        }

        switch (await RemoveFromStorageUntilEmptyOfItem(storageGridContainer, rawArray, rawItem))
        {
            case 0:
                await SaveData.SyncInventory();
                return true;

            case > 0:
                GD.PrintErr("Didn't remove enough items from inventory! @PlayerInventoryController.cs");
                return false;

            case < 0:
                GD.PrintErr("Removed too many items from inventory! @PlayerInventoryController.cs");
                return false;
        }
    }

    /// <summary>
    /// Removes quantity of item from inventory as long as it is possible
    /// </summary>
    /// <param name="rawArray"></param>
    /// <param name="itemToRemove"></param>
    /// <param name="slotContainer"></param>
    /// <returns> How many could not be removed </returns>
    static async Task<int> RemoveFromStorageUntilEmptyOfItem(
        GridContainer slotContainer, Array<RawInventoryItem> rawArray, RawInventoryItem itemToRemove)
    {
        int amountToRemove = itemToRemove.quantity;

        for (int i = rawArray.Count - 1; i >= 0; i--)
        {
            if (rawArray[i] == null) continue;

            if (rawArray[i].id == itemToRemove.id)
            {
                amountToRemove = await RemoveFromSlotUntilEmpty(slotContainer, rawArray, amountToRemove, i);
            }

            if (amountToRemove == 0) break;
        }

        return amountToRemove;
    }

    /// <summary>
    /// Removes items from inventory slot until all are removed OR slot's item quantity reaches 0.
    /// </summary>
    /// <param name="rawArray"></param>
    /// <param name="amountToRemove"></param>
    /// <param name="index"> must be valid </param>
    /// <param name="mustRemoveAll"> (optional) will not remove anything if itemToRemove.quantity is greater than quantity in slot. </param>
    /// <param name="slotContainer"></param>
    /// <returns> amount that couldn't be removed </returns>
    static Task<int> RemoveFromSlotUntilEmpty(GridContainer slotContainer, Array<RawInventoryItem> rawArray,
        int amountToRemove, int index, bool mustRemoveAll = false)
    {
        if (index >= rawArray.Count) return Task.FromResult(amountToRemove);

        RawInventoryItem itemInSlot = rawArray[index];

        int amountRemoved = (itemInSlot.quantity - amountToRemove > 0)
            ? amountToRemove
            : itemInSlot.quantity;

        if (mustRemoveAll && amountToRemove != amountRemoved) return Task.FromResult(amountToRemove);

        itemInSlot.quantity -= amountRemoved;

        HandleRemainderOfAddOrRemoveOperation(itemInSlot);

        return Task.FromResult(amountToRemove - amountRemoved);
    }

    
    
    
    // UPDATING SLOTS
    
    
    static void HandleRemainderOfAddOrRemoveOperation(RawInventoryItem itemToHandle)
    {
        bool slotEmpty = itemToHandle.quantity == 0;

        if (PlayerInventoryController.HasSameItemSelected(itemToHandle))
        {
            if (slotEmpty)
            {
                PlayerInventoryController.DeselectItem();
            }
            else
            {
                PlayerInventoryController.UpdateSelectedItemVisuals();
            }
        }

        if (!itemToHandle.HasValidIndexInArray()) return;

        if (slotEmpty)
        {
            NullifyItemAtIndex(itemToHandle.hostGrid, itemToHandle.hostArray, itemToHandle.indexInStorageArray);
        }
        else
        {
            UpdateStorageSlot(itemToHandle.hostGrid, itemToHandle.hostArray, itemToHandle,
                itemToHandle.indexInStorageArray);
        }
    }

    public static void NullifyItemAtIndex(GridContainer slotContainer, Array<RawInventoryItem> rawArray, int index)
    {
        UpdateStorageSlot(slotContainer, rawArray, null, index);
    }

    public static void UpdateStorageSlot(GridContainer slotContainer, Array<RawInventoryItem> rawArray,
        RawInventoryItem item, int index, bool doSync = true)
    {
        UpdateSlot(slotContainer, rawArray, item, index, doSync);
    }

    static async void UpdateSlot(GridContainer slotContainer, Array<RawInventoryItem> rawArray,
        RawInventoryItem rawItem, int index, bool doSync = true)
    {
        var slotToUpdate = slotContainer.GetChild<StorageSlot>(index);

        await UpdateSlotAsync(slotToUpdate, rawArray, rawItem, doSync);
    }

    static async Task UpdateSlotAsync(StorageSlot slot, Array<RawInventoryItem> rawArray, RawInventoryItem rawItem,
        bool doSync = true)
    {
        await TaskExtensions.SuspendWhile(() => !slot.slotInitialized);

        slot.ToggleVisuals(true);

        slot.slotItem = rawItem;

        rawArray[slot.slotIndex] = slot.slotItem;


        if (slot.slotItem != null)
        {
            rawItem.indexInStorageArray = slot.slotIndex;

            if (PlayerInventoryController.HasSameItemSelected(rawItem))
            {
                PlayerInventoryController.SelectNewItem(slot.slotItem);
            }

            if (slot.slotItem.quantity == 0)
            {
                GD.PushError("Somehow ended up with 0 quantity storage slot");
            }

            slot.slotItem.hostSlotType = slot.slotType;
            slot.slotItem.hostArray = rawArray;
            slot.slotItem.hostGrid = slot.GetParent() as GridContainer;
        }

        slot.UpdateVisuals();

        if (doSync)
        {
            Task sync = SaveData.SyncInventory();
        }
    }

    
    
    
    // INITIALIZING
    
    public static void InitializeItemGridContainer(GridContainer slotContainer, Array<RawInventoryItem> rawArray,
        StorageSlotType slotType, int startIndex, int lastIndex)
    {
        foreach (var node in slotContainer.GetChildren())
        {
            node.Free();
        }

        string resourcePath = slotType switch
        {
            StorageSlotType.PlayerInventory => INVENTORY_SLOT_RESPATH,
            StorageSlotType.Hotbar => HOTBAR_SLOT_RESPATH,
            StorageSlotType.TownStorage => TOWN_STORAGE_SLOT_RESPATH,
            _ => throw new ArgumentOutOfRangeException(nameof(slotType), slotType, null)
        };

        InitSlotsWithItems(slotContainer, rawArray, resourcePath, startIndex, lastIndex);
    }

    static void InitSlotsWithItems(GridContainer slotContainer, Array<RawInventoryItem> rawArray,
        string packedSceneString, int startIndex, int lastIndex)
    {
        var itemSlotNode = GD.Load<PackedScene>(packedSceneString);

        for (int i = startIndex; i <= lastIndex; i++)
        {
            var itemSlot = itemSlotNode.Instantiate<StorageSlot>();

            slotContainer.AddChild(itemSlot);

            itemSlot.InitializeSlot(i);

            UpdateSlot(slotContainer, rawArray, rawArray[i], i, false);
        }
    }

    
    
    
    //SELECTING
    
    public static void SelectSingleItem(GridContainer slotContainer, Array<RawInventoryItem> rawArray,
        RawInventoryItem item, int index)
    {
        item.quantity -= 1;

        if (item.quantity >= 1)
        {
            UpdateStorageSlot(slotContainer, rawArray, item, index);
        }
        else
        {
            NullifyItemAtIndex(slotContainer, rawArray, index);
        }


        PlayerInventoryController.SelectNewItem(new RawInventoryItem(item.id, item.name, 1, item.stackSize));
    }

    public static void SelectItemAtSlot(GridContainer slotContainer, int index)
    {
        StorageSlot slot = slotContainer.GetChild<StorageSlot>(index);

        if (!slot.hasItem)
        {
            GD.Print("Can't select an item in slot with no item");
            PlayerInventoryController.DeselectItem();
            return;
        }

        PlayerInventoryController.SelectNewItem(slot.slotItem);
        slot.ToggleVisuals(false);
    }
}