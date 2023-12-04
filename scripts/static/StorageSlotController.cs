
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public enum StorageSlotType
{
	Uninitialized = 0,
    PlayerInventory = 10,
    Hotbar = 20,
    TownStorage = 30,
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
        
		rawArray[slot.slotIndex] = slot.slotItem;
		
		

		if (slot.slotItem != null)
		{
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
}