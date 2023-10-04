using Godot;
using System;

public partial class InventorySlot : Control {
	
	public TextureRect icon;
	public Label quantityLabel;
	private PlayerInventoryController _inv;

	public int itemID = -1;
	public int quantity = 0;
	public int index = -1;


	public override void _Ready() {
		_inv = GetNode<PlayerInventoryController>("../..") as PlayerInventoryController;
		icon = GetNode("Icon") as TextureRect;
		quantityLabel = GetNode("Quantity") as Label;
	}


	private void OnButtonGuiInput(InputEvent @event) {
		if (@event is InputEventMouseButton keyEvent && keyEvent.Pressed) {
			if (keyEvent.ButtonIndex == MouseButton.Left) {
				_inv.SelectItem((int)itemID);
			}
		}
	}


	public void UpdateSlot(Variant _receivedItemID, Variant _itemQuantity, int _itemIndex) {
		itemID = (int)_receivedItemID;
		quantity = (int)_itemQuantity;
		index = _itemIndex;

		GD.Print(itemID);

		if (itemID == -1) {
			icon.Texture = null;
			return;
		}

		var itemResource = ItemData.items[itemID] as Item;

		Texture2D iconTexture = itemResource.IconTexture;
		icon.Texture = iconTexture;
	}
}
