
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public static class StorageController
{
	const string droppedItemNodePath = "res://scenes/world/dropped_item.tscn";
    public const string INVENTORY_SLOT_RESPATH = "res://scenes/ui/inventory/inventory_slot.tscn";

    ///  <summary> Main Storage additive operation </summary>
    ///  <param name="storageGridContainer"></param>
    ///  <param name="rawArray"> The array of RawInventoryItem to add the item to</param>
    ///  <param name="rawItem"> Includes id, name and quantity to add </param>
    ///  <param name="index"> optional: index in the inventory array </param>
    ///  <param name="affectSelectedItem"> optional : affect the current selected item </param>
    ///  <param name="deselectOnAllAdded"> optional : deselect current item if all items were added </param>
    ///  
    /// 	<returns> how many items could *NOT* be added </returns>
    public static async Task<int> AddItem(GridContainer storageGridContainer, Array<RawInventoryItem> rawArray,
	    RawInventoryItem rawItem, int index = -1, bool affectSelectedItem = false, bool deselectOnAllAdded = true)
	{

		if (rawItem == null)
		{
			GD.PrintErr("Tried to add a null item to inventory @StorageController.AddItem!");
			return -1;
		}
		
		rawItem.quantity = index == -1 
			? await AddToGridUntilFull(storageGridContainer, rawArray, rawItem) // index not specified
			: await AddToSlotUntilFull(storageGridContainer, rawArray, rawItem, index); // index given

		bool indexValid = rawItem.HasValidIndexInArray(rawArray);
		
		if (rawItem.quantity == 0)
		{
			if (indexValid)
			{
				NullifyItemAtIndex(storageGridContainer, rawArray, rawItem.indexInStorage);
			}
		}
		
		
        if (affectSelectedItem && PlayerInventoryController.isItemSelected)
        {
	        if (rawItem.quantity == 0)
	        {
		        // All items in question added
		        if (deselectOnAllAdded)
		        {
			        PlayerInventoryController.DeselectItem();
		        }
	        }
	        else if (PlayerInventoryController.selectedItem.id == rawItem.id && indexValid)
	        {
		        PlayerInventoryController.SelectInventoryItemAtSlot(
			        PlayerInventoryController.selectedItem.indexInStorage);
	        }
        }
        
        await SaveData.SyncInventory();
        return rawItem.quantity;
	}

	static async Task<int> AddToGridUntilFull(GridContainer storageGridContainer, Array<RawInventoryItem> rawArray,
		RawInventoryItem itemToAdd)
	{
		for (int i = 0; i < rawArray.Count; i++)
		{
			RawInventoryItem itemInSlot = rawArray[i];
            
			if (itemInSlot == null || itemToAdd.id == itemInSlot.id)
			{
				itemToAdd.quantity = await AddToSlotUntilFull(storageGridContainer, rawArray, itemToAdd, i);
			}

			if (itemToAdd.quantity == 0) break;
		}
		
		return itemToAdd.quantity;
	}
	
	static Task<int> AddToSlotUntilFull(GridContainer storageGridContainer, Array<RawInventoryItem> rawArray,
		RawInventoryItem itemToAdd, int index)
	{
		RawInventoryItem itemInSlot = rawArray[index];
        
		int spaceRemainingAtIndex = itemInSlot?.SpaceRemainingInStack ?? itemToAdd.stackSize;
		
		int amountToAdd = itemToAdd.quantity;

		int howManyWereAdded = spaceRemainingAtIndex - amountToAdd < 0
			? spaceRemainingAtIndex
			: amountToAdd;

		if (howManyWereAdded > 0)
		{
			RawInventoryItem addedItem = new RawInventoryItem(
				itemToAdd.id,
				itemToAdd.name,
				itemToAdd.stackSize - spaceRemainingAtIndex + howManyWereAdded,
				itemToAdd.stackSize);
			
			UpdateStorageSlot(storageGridContainer, rawArray, addedItem, index);

			itemToAdd.quantity -= howManyWereAdded;
		}
        
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
			if (index <= rawArray.Count - 1)
			{
				return await RemoveFromSlotUntilEmpty(storageGridContainer, rawArray, rawItem.quantity, index, true) == 0;
			}
			
			GD.PrintErr("Index was greater than player inventory max size - 1");
			return false;
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
		RawInventoryItem itemInSlot = rawArray[index];
				
		int amountRemoved = (itemInSlot.quantity - amountToRemove > 0)
			? amountToRemove
			: itemInSlot.quantity;

		if (mustRemoveAll && amountToRemove != amountRemoved) return Task.FromResult(amountToRemove);
		
		itemInSlot.quantity -= amountRemoved;
        
		if (itemInSlot.quantity > 0)
		{
            UpdateStorageSlot(slotContainer, rawArray, itemInSlot, index);
		}
		else
		{
			NullifyItemAtIndex(slotContainer, rawArray, index);
		}
        
		return Task.FromResult(amountToRemove - amountRemoved);
	}
	
	
    public static void NullifyItemAtIndex(GridContainer slotContainer, Array<RawInventoryItem> rawArray, int index)
	{
		UpdateStorageSlot(slotContainer, rawArray, null, index);
	}

	public static async void UpdateStorageSlot(GridContainer slotContainer, Array<RawInventoryItem> rawArray, RawInventoryItem item, int index)
	{
		var slotToUpdate = slotContainer.GetChild<InventorySlot>(index);
        StorageSlotController.UpdateSlot(slotToUpdate, rawArray, item);
	}
}