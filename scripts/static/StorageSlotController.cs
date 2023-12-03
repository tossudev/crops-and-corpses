
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public enum StorageSlotType
{
    PlayerInventory,
    TownStorage,
    Chest
}

public static class StorageSlotController
{
    
	public static async void UpdateSlot(StorageSlot slot, Array<RawInventoryItem> rawArray, RawInventoryItem rawItem, bool doSync = true)
	{
		await UpdateSlotAsync(slot, rawArray, rawItem, doSync);
	}
	
    static async Task UpdateSlotAsync (StorageSlot slot, Array<RawInventoryItem> rawArray, RawInventoryItem rawItem, bool doSync = true)
	{
		await TaskExtensions.SuspendWhile(() => !slot.slotInitialized);
		
		slot.ToggleVisuals(true);

		slot.slotItem = (rawItem != null)
			? new RawInventoryItem(rawItem.id, rawItem.name, rawItem.quantity, rawItem.stackSize, slot.slotIndex)
			: null;
        
		if (rawArray.Count > slot.slotIndex)
		{
			rawArray[slot.slotIndex] = slot.slotItem;
		}
		else
		{
			rawArray.Add(slot.slotItem);
		}
		
		if (PlayerInventoryController.isItemSelected)
		{
			if (PlayerInventoryController.selectedItem.indexInStorage == slot.slotIndex)
			{
				if (slot.slotItem == null)
				{
					PlayerInventoryController.DeselectItem();
				}
				else
				{
					PlayerInventoryController.SelectNewItem(slot.slotItem);
				}
			}
		}
        
		if (doSync)
		{
			Task sync = SaveData.SyncInventory();
		}
		
		slot.UpdateVisuals();
	}
}