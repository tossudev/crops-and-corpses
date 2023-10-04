using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System.Collections.Generic;

public partial class PlayerInventoryController : Control {

	public TextureRect selectedIcon;
	public Label selectedQuantityLabel;
	public bool isItemSelected = false;
	private string slotNodePath = "res://scenes/ui/inventory_slot.tscn";
	private Control _selectedItemNode;
	private const int SELECTED_ITEM_OFFSET = 32;


	public override void _Ready() {
		_InitInventory();
		_selectedItemNode = GetNode("SelectedItem") as Control;
		selectedIcon = GetNode("SelectedItem/Icon") as TextureRect;
		selectedQuantityLabel = GetNode("SelectedItem/Quantity") as Label;
	}


    public override void _Process(double delta) {
        _UpdateSelectedItem();
    }


	private void _UpdateSelectedItem() {
		Vector2 _mousePosition = GetGlobalMousePosition();
		_mousePosition.X -= SELECTED_ITEM_OFFSET;
		_mousePosition.Y -= SELECTED_ITEM_OFFSET;

		_selectedItemNode.GlobalPosition = _mousePosition;
		_selectedItemNode.Visible = isItemSelected;
	}


	public void SelectItem(int ItemID) {
		if (ItemID == -1) {
			return;
		}

		isItemSelected = true;

		var _itemResource = ItemData.items[ItemID] as Item;

		Texture2D _iconTexture = _itemResource.IconTexture;
		selectedIcon.Texture = _iconTexture;
	}


    private void _InitInventory() {

		int _itemIndex = 0;
		foreach (Dictionary itemResource in PlayerInventoryData.PlayerInventory) {
			var itemSlotNode = GD.Load<PackedScene>(slotNodePath);
			var itemSlot = itemSlotNode.Instantiate<Control>();

			// GD.Print(itemResource);

			// var slot = ResourceLoader.Load<PackedScene>(slotNodePath).Instantiate();
			// GetNode<GridContainer>("InventoryGrid").AddChild(slot);

			GetNode<GridContainer>("InventoryGrid").AddChild(itemSlot);
			
			int _itemID = (int)itemResource.GetValueOrDefault("ID");
			int _itemQuantity = (int)itemResource.GetValueOrDefault("Quantity");

			// int newItemID = VariantUtils.ConvertTo<int>(itemID);
			// int newItemID = VariantUtils.ConvertTo<int>(itemID);
			// int newnewItemID = itemID;

			((InventorySlot)itemSlot).UpdateSlot(_itemID, _itemQuantity, _itemIndex);

			_itemIndex ++;
		}
	}
}
