using Godot;
using System;

public partial class InventorySlot : Control {
	
	public TextureRect Icon;
	public Label QuantityLabel;

	// TODO: Convert Variants to int in PlayerInventoryController.cs
	// ^ Not necessary, can be converted with cast "(int)"
	public int ItemID;
	public int Quantity;
	public int Index;

	[Export] public Texture2D emptySlotTexture;

	public override void _Ready() {
		Icon = GetNode("Icon") as TextureRect;
		QuantityLabel = GetNode("Quantity") as Label;
	}


	public void UpdateSlot(Variant itemID, Variant itemQuantity, int itemIndex) {
		ItemID = (int)itemID;
		Index = itemIndex;

		if (ItemID == -1)
		{
			// Slot is empty
			Icon.Texture = emptySlotTexture;
			QuantityLabel.Text = "";
			return;
		}
		
		Quantity = (int)itemQuantity;

		var itemResource = ItemData.Items[ItemID];

		Texture2D iconTexture = itemResource.IconTexture;
		Icon.Texture = iconTexture;

		QuantityLabel.Text = Quantity.ToString();
	}
}
