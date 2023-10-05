using Godot;
using System;

public partial class InventorySlot : Control {
	
	public TextureRect icon;
	public Label quantityLabel;

	public int itemID = -1;
	public int quantity = 0;
	public int index = -1;
	private PlayerInventoryController _inv;
	private bool hasItem = false;


	public override void _Ready() {
		try
		{
			_inv = GetNode<PlayerInventoryController>("../..") as PlayerInventoryController;
		}
		catch (Exception e)
		{
			GD.Print("No inventory controller for InventorySlot");
		}
		
		icon = GetNode("Icon") as TextureRect;
		quantityLabel = GetNode("Quantity") as Label;
	}


	private void OnButtonGuiInput(InputEvent @event) {
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) {
			if (mouseEvent.ButtonIndex == MouseButton.Left) {
				ClickLeft();
			}
		}
	}


	private void ClickLeft() {
		// Player deselected item from hand
		if (_inv.isItemSelected && !hasItem) {
			_inv.AddItem(index);
		}

		// Player selects new item
		else if (!_inv.isItemSelected && hasItem) {
			_inv.SelectItem(itemID, quantity);
			_inv.RemoveAtIndex(index);
		}

		// Player has item and presses new item
		else if (_inv.isItemSelected && hasItem) {
			if (DoItemsMatch()) {
				_inv.AddItem(index);
			}
		}
	}

	private bool DoItemsMatch() {
		if (itemID == _inv.selectedItemID) {
			return true;
		}
		
		return false;
	}


	public void UpdateSlot(int _receivedItemID, int _itemQuantity, int _itemIndex) {
		itemID = _receivedItemID;
		quantity = _itemQuantity;
		index = _itemIndex;

		// GD.Print(itemID);

		if (itemID == -1) {
			hasItem = false;

			icon.Texture = null;
			quantityLabel.Text = "";
			return;
		}

		hasItem = true;
		var itemResource = ItemData.items[itemID] as Item;

		Texture2D iconTexture = itemResource.IconTexture;
		icon.Texture = iconTexture;
		quantityLabel.Text = quantity.ToString();
	}
}
