using Godot;
using System;
using System.Threading.Tasks;

public partial class InventorySlot : StorageSlot {
	
	
	
    void OnButtonGuiInput(InputEvent @event)
	{
		if (isCraftingSlot) return;
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) {
			if (mouseEvent.ButtonIndex == MouseButton.Left) {
				ClickLeft();
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Right) {
				ClickRight();
			}
		}
	}


    protected override void ClickLeft()
    {
	    switch (PlayerInventoryController.isItemSelected)
	    {
		    // Player selects new item
		    case false when slotHasItem:
			    PlayerInventoryController.SelectNewItem(slotItem);
			    ToggleVisuals(false);
			    break;
		    
		    // Player deselected item from hand
		    case true when !slotHasItem:
                AddItemToInventory();
				break;
		    
		    // Player has item a slot with item
		    case true when slotHasItem:
		    {
			    var selectedItem = PlayerInventoryController.selectedItem;
			    
			    if (selectedItem.HasValidIndexInArray(SaveData.organizedPlayerInventory) &&
			        slotIndex == selectedItem.indexInStorage)
			    {
				    StorageSlotController.UpdateSlot(this, SaveData.organizedPlayerInventory, PlayerInventoryController.selectedItem);
				    PlayerInventoryController.DeselectItem();
			    }

			    else if (slotItem.id == selectedItem.id)
			    {
				    AddItemToInventory();
			    }

			    else {
				    PlayerInventoryController.SwapItems(slotItem, slotIndex);
			    }

			    break;
		    }
	    }
    }


    protected override void ClickRight() {
		// Player takes one item from stack
		if (!PlayerInventoryController.isItemSelected && slotHasItem) {
			
			PlayerInventoryController.SelectSingleItem(slotItem, slotIndex);
		}
	}


    async void AddItemToInventory()
    {
	    await PlayerInventoryController.AddItemToInventory(PlayerInventoryController.selectedItem, slotIndex, true);
    }
    
    public override void ToggleVisuals(bool on)
    {
	    icon.Visible = on;
	    quantityLabel.Visible = on;
    }
}
