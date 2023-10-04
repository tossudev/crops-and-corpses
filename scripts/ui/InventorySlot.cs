using Godot;
using System;

public partial class InventorySlot : Control {
	
	public TextureRect Icon;
	public Label QuantityLabel;
	private PlayerInventoryController _Inv;

	// TODO: Convert Variants to int in PlayerInventoryController.cs
	public int ItemID = -1;
	public Variant Quantity = 0;
	public int Index = -1;


	public override void _Ready() {
		_Inv = GetNode<PlayerInventoryController>("../..") as PlayerInventoryController;
		Icon = GetNode("Icon") as TextureRect;
		QuantityLabel = GetNode("Quantity") as Label;
	}


	private void OnButtonGuiInput(InputEvent @event) {
		if (@event is InputEventMouseButton keyEvent && keyEvent.Pressed) {
			if (keyEvent.ButtonIndex == MouseButton.Left) {
				_Inv.SelectItem((int)ItemID);
			}
		}
	}


	public void UpdateSlot(Variant itemID, Variant itemQuantity, int itemIndex) {
		ItemID = (int)itemID;
		Quantity = itemQuantity;
		Index = itemIndex;

		GD.Print(ItemID);

		// If there is no item, clear texture
		if (ItemID == -1) {
			Icon.Texture = null;
			return;
		}

		var itemResource = ItemData.Items[ItemID] as Item;

		Texture2D iconTexture = itemResource.IconTexture;
		Icon.Texture = iconTexture;
	}
}
