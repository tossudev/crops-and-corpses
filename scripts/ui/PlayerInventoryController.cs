using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System.Collections.Generic;

public partial class PlayerInventoryController : Control {

	public TextureRect selectedIcon;
	public Label selectedQuantityLabel;
	public bool isItemSelected = false;
	public int selectedItemID = -1;
	public int selectedItemQuantity = 0;
	public bool isOpen = false;

	private string slotNodePath = "res://scenes/ui/inventory_slot.tscn";
	private Control _selectedItemNode;
	private const int SELECTED_ITEM_OFFSET = 64;


	public override void _Ready() {
		_InitInventory();
		_selectedItemNode = GetNode("SelectedItem") as Control;
		selectedIcon = GetNode("SelectedItem/Icon") as TextureRect;
		selectedQuantityLabel = GetNode("SelectedItem/Quantity") as Label;
	}


    public override void _Process(double delta) {
        _UpdateSelectedItem();
    }


	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("toggle_inventory")) {
			isOpen = !isOpen;
			Visible = isOpen;
		}
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


	private void _UpdateSelectedItem() {
		Vector2 _mousePosition = GetGlobalMousePosition();
		_mousePosition.X -= SELECTED_ITEM_OFFSET;
		_mousePosition.Y -= SELECTED_ITEM_OFFSET;

		_selectedItemNode.GlobalPosition = _mousePosition;
		_selectedItemNode.Visible = isItemSelected;
	}


	public void SelectItem(int _itemID, int _itemQuantity) {
		if (_itemID == -1) {
			return;
		}

		isItemSelected = true;
		selectedItemID = _itemID;
		selectedItemQuantity = _itemQuantity;

		var _itemResource = ItemData.items[_itemID] as Item;

		Texture2D _iconTexture = _itemResource.IconTexture;
		selectedIcon.Texture = _iconTexture;
		selectedQuantityLabel.Text = selectedItemQuantity.ToString();
	}


	public void AddItem(int _index) {
		Dictionary _previousItem = (Dictionary)PlayerInventoryData.PlayerInventory[_index];
		int _previousItemQuantity = (int)_previousItem["Quantity"];

		int _newQuantity = _previousItemQuantity + selectedItemQuantity;


		var _newItem = new Godot.Collections.Dictionary<string, Variant>();
        _newItem.Add("ID", selectedItemID);
        _newItem.Add("Quantity", _newQuantity);

		PlayerInventoryData.PlayerInventory[_index] = _newItem;
		Control slotToAdd = GetNode<Control>("InventoryGrid").GetChild<Control>(_index);
		((InventorySlot)slotToAdd).UpdateSlot(selectedItemID, _newQuantity, _index);
	
		DeselectItem();
	}


	public void RemoveAtIndex(int _index) {
		var _emptyItem = new Godot.Collections.Dictionary<string, Variant>();
        _emptyItem.Add("ID", -1);
        _emptyItem.Add("Quantity", 0);

		PlayerInventoryData.PlayerInventory[_index] = _emptyItem;
		Control slotToRemove = GetNode<Control>("InventoryGrid").GetChild<Control>(_index);
		((InventorySlot)slotToRemove).UpdateSlot(-1, 0, _index);
	}


	public void SelectSingleItem(int index, int itemID, int quantity) {
		int _remainingQuantity = quantity - 1;

		if (_remainingQuantity >= 1) {
			SelectItem(itemID, _remainingQuantity);
			RemoveAtIndex(index);
			AddItem(index);
		}
		else {
			RemoveAtIndex(index);
		}

		SelectItem(itemID, 1);
	}


	public void SwapItems(int index, int itemID, int quantity) {
		SelectItem(selectedItemID, selectedItemQuantity);
		RemoveAtIndex(index);
		AddItem(index);
		SelectItem(itemID, quantity);
	}


	public void DeselectItem() {
		selectedItemID = -1;
		selectedItemQuantity = 0;
		isItemSelected = false;
	}
}
