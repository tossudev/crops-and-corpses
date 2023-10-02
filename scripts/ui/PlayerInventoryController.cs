using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System.Collections.Generic;

public partial class PlayerInventoryController : Control {

	private string slotNodePath = "res://scenes/ui/inventory_slot.tscn";
	public override void _Ready() {
		_InitInventory();
	}


	private void _InitInventory() {

		int itemIndex = 0;
		foreach (Dictionary itemResource in PlayerInventoryData.PlayerInventory) {
			var itemSlotNode = GD.Load<PackedScene>(slotNodePath);
			var itemSlot = itemSlotNode.Instantiate<Control>();

			// GD.Print(itemResource);

			// var slot = ResourceLoader.Load<PackedScene>(slotNodePath).Instantiate();
			// GetNode<GridContainer>("InventoryGrid").AddChild(slot);

			GetNode<GridContainer>("InventoryGrid").AddChild(itemSlot);
			
			Variant itemID = itemResource.GetValueOrDefault("ID");
			Variant itemQuantity = itemResource.GetValueOrDefault("Quantity");

			// int newItemID = VariantUtils.ConvertTo<int>(itemID);
			// int newItemID = VariantUtils.ConvertTo<int>(itemID);
			// int newnewItemID = itemID;

			((InventorySlot)itemSlot).UpdateSlot(itemID, itemQuantity, itemIndex);

			itemIndex ++;
		}
	}
}
