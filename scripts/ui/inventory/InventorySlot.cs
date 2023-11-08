using Godot;
using System;
using System.Threading.Tasks;

public partial class InventorySlot : Control {
	
	public TextureRect icon;
	public Label quantityLabel;

	public RawInventoryItem slotItem;
	[Export] public bool isCraftingSlot;
	bool slotHasItem;
	public bool slotInitiated;
    int _slotIndex;
	public int slotIndex => _slotIndex;


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


    async void ClickLeft()
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
                await PlayerInventoryController.AddItem(PlayerInventoryController.selectedItem, slotIndex, true);
                
				break;
		    
		    // Player has item a slot with item
		    case true when slotHasItem:
		    {
			    var selectedItem = PlayerInventoryController.selectedItem;
			    
			    if (selectedItem.isValidIndex &&
			        slotIndex == selectedItem.indexInOrganizedInventory)
			    {
				    await UpdateSlot(selectedItem);
				    PlayerInventoryController.DeselectItem();
			    }

			    else if (slotItem.id == selectedItem.id) {
				    await PlayerInventoryController.AddItem(selectedItem, slotIndex, true);
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

    public void InitiateSlot(int index)
    {
	    icon = GetNode("Icon") as TextureRect;
	    quantityLabel = GetNode("Quantity") as Label;
	    
	    _slotIndex = index;

	    slotInitiated = true;
    }
    
	public async Task UpdateSlot(RawInventoryItem rawItem, bool doSync = true)
	{
		await TaskExtensions.SuspendWhile(() => !slotInitiated);
		
		ToggleVisuals(true);

		slotItem = (rawItem != null)
			? new RawInventoryItem(rawItem.id, rawItem.name, rawItem.quantity, rawItem.stackSize, slotIndex)
			: null;
        
		if (SaveData.organizedPlayerInventory.Count > slotIndex)
		{
			SaveData.organizedPlayerInventory[slotIndex] = slotItem;
		}
		else
		{
			SaveData.organizedPlayerInventory.Add(slotItem);
		}
		
		if (PlayerInventoryController.isItemSelected)
		{
			if (PlayerInventoryController.selectedItem.indexInOrganizedInventory == slotIndex)
			{
				if (slotItem == null)
				{
					PlayerInventoryController.DeselectItem();
				}
				else
				{
					PlayerInventoryController.SelectItem(slotItem);
				}
			}
		}
        
		if (doSync)
		{
			Task sync = SaveData.SyncInventory();
		}
		
		if (slotItem == null) {
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
