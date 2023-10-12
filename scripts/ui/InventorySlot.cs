using Godot;
using System;

public partial class InventorySlot : Control {
	
	public TextureRect icon;
	public Label quantityLabel;

	public RawInventoryItem slotItem;
	[Export] public bool isCraftingSlot;
	bool hasItem;
	public int index = -1;


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


    void ClickLeft() {
		// Player deselected item from hand
		if (PlayerInventoryController.isItemSelected && !hasItem) {
			PlayerInventoryController.AddItem(PlayerInventoryController.selectedItem, index);
		}

		// Player selects new item
		else if (!PlayerInventoryController.isItemSelected && hasItem)
		{
			PlayerInventoryController.SelectItem(new RawInventoryItem(
				slotItem.id, slotItem.name, slotItem.quantity, slotItem.stackSize));
			
			PlayerInventoryController.NullifyInventoryItemAtIndex(index);
		}

		// Player has item and presses new item
		else if (PlayerInventoryController.isItemSelected && hasItem) {
			if (slotItem.id == PlayerInventoryController.selectedItem.id) {
				PlayerInventoryController.AddItem(PlayerInventoryController.selectedItem, index);
			}

			else {
				PlayerInventoryController.SwapItems(slotItem, index);
			}
		}
	}


    void ClickRight() {
		// Player takes one item from stack
		if (!PlayerInventoryController.isItemSelected && hasItem) {
			PlayerInventoryController.SelectSingleItem(slotItem, index);
		}
	}
    


	public void UpdateSlot(RawInventoryItem rawItem, int itemIndex)
	{
		slotItem = (rawItem != null) 
			? new RawInventoryItem(rawItem.id, rawItem.name, rawItem.quantity, rawItem.stackSize, itemIndex) 
			: null;
		
		index = itemIndex;
        
		if (SaveData.organizedPlayerInventory.Count > index)
		{
			SaveData.organizedPlayerInventory[index] = slotItem;
		}
		else
		{
			SaveData.organizedPlayerInventory.Add(slotItem);
		}
		
		if (rawItem == null) {
			hasItem = false;

			icon.Texture = null;
			quantityLabel.Text = "";
			return;
		}

		hasItem = true;
		
		Item itemResource = ItemData.GetItemById(rawItem.id);

		Texture2D iconTexture = itemResource.IconTexture;
		icon.Texture = iconTexture;

		quantityLabel.Visible = itemResource.StackSize > 1;
		quantityLabel.Text = rawItem.quantity.ToString();
	}
}
