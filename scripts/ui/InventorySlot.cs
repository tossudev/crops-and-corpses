using Godot;
using System;

public partial class InventorySlot : Control {
	
	public TextureRect Icon;
	public Label QuantityLabel;

	// TODO: Convert Variants to int in PlayerInventoryController.cs
	public Variant ItemID = -1;
	public Variant Quantity = 0;
	public int Index = -1;


	public override void _Ready() {
		Icon = GetNode("Icon") as TextureRect;
		QuantityLabel = GetNode("Quantity") as Label;
	}


	public void UpdateSlot(Variant itemID, Variant itemQuantity, int itemIndex) {
		ItemID = itemID;
		Quantity = itemQuantity;
		Index = itemIndex;

		// var itemResource = ItemData<ItemData>.Items[ItemID];
		var itemResource = ItemData.Items[0] as Item;

		Texture2D iconTexture = itemResource.IconTexture;
		Icon.Texture = iconTexture;
	}
}
