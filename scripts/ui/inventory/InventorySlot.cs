using Godot;
using System;

public partial class InventorySlot : Control {
	
	public TextureRect icon;
	public Label quantityLabel;

	public RawInventoryItem slotItem;
	[Export] public bool isCraftingSlot;
	bool slotHasItem;
	public int slotIndex = -1;


	public override void _Ready() {
		icon = GetNode("Icon") as TextureRect;
		quantityLabel = GetNode("Quantity") as Label;
	}


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


    void ClickLeft()
    {
	    switch (PlayerInventoryController.isItemSelected)
	    {
		    // Player selects new item
		    case false when slotHasItem:
			    PlayerInventoryController.SelectItem(slotItem);
			    ToggleVisuals(false);
			    break;
		    
		    // Player deselected item from hand
		    case true when !slotHasItem:
                PlayerInventoryController.AddItem(PlayerInventoryController.selectedItem, slotIndex, true);
                
				break;
		    
		    // Player has item a slot with item
		    case true when slotHasItem:
		    {
			    if (PlayerInventoryController.selectedItem.isValidIndex &&
			        slotIndex == PlayerInventoryController.selectedItem.indexInOrganizedInventory)
			    {
				    ToggleVisuals(true);
				    PlayerInventoryController.DeselectItem();
			    }

			    else if (slotItem.id == PlayerInventoryController.selectedItem.id) {
				    PlayerInventoryController.AddItem(PlayerInventoryController.selectedItem, slotIndex, true);
			    }

			    else {
				    PlayerInventoryController.SwapItems(slotItem, slotIndex);
			    }

			    break;
		    }
	    }
    }


    void ClickRight() {
		// Player takes one item from stack
		if (!PlayerInventoryController.isItemSelected && slotHasItem) {
			PlayerInventoryController.SelectSingleItem(slotItem, slotIndex);
		}
	}


    public void ToggleVisuals(bool isOn)
    {
	    icon.Visible = isOn;
	    quantityLabel.Visible = isOn;
    }

	public void UpdateSlot(RawInventoryItem rawItem, int itemIndex, bool doSync = true)
	{
		ToggleVisuals(true);
		slotItem = (rawItem != null) 
			? new RawInventoryItem(rawItem.id, rawItem.name, rawItem.quantity, rawItem.stackSize, itemIndex) 
			: null;
		
		slotIndex = itemIndex;
        
		if (SaveData.organizedPlayerInventory.Count > slotIndex)
		{
			SaveData.organizedPlayerInventory[slotIndex] = slotItem;
		}
		else
		{
			SaveData.organizedPlayerInventory.Add(slotItem);
		}

		if (doSync)
		{
			SaveData.SyncInventory();
		}
		
		if (rawItem == null) {
			slotHasItem = false;

			icon.Texture = null;
			quantityLabel.Text = "";
			return;
		}

		slotHasItem = true;
		
		Item itemResource = ItemData.GetItemById(rawItem.id);

		Texture2D iconTexture = itemResource.IconTexture;
		icon.Texture = iconTexture;

		quantityLabel.Visible = itemResource.StackSize > 1;
		quantityLabel.Text = rawItem.quantity.ToString();
	}
}
