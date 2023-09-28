using Godot;
using System;

public partial class PlayerInventoryController : Control {

	private string slotNodePath = "res://scenes/ui/inventory_slot.tscn";
	public override void _Ready() {
		_InitInventory();
	
	}


	private void _InitInventory() {
		foreach (var item in PlayerInventoryData.PlayerInventory) {
			var slot = ResourceLoader.Load<PackedScene>(slotNodePath).Instantiate();
			GetNode<GridContainer>("InventoryGrid").AddChild(slot);
			// GD.Print(item);
		}
	}
}
