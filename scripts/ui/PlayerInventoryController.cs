using Godot;
using System.Threading.Tasks;

public partial class PlayerInventoryController : Control {

	public static TextureRect selectedIcon;
	public static Label selectedQuantityLabel;
	public static Sprite2D heldItemIndicator;

	static Control _inventoryPanel;
	static Control _inventoryGrid;
	static Control _hotbarPanel;
	static Control _hotbarGrid;
	
	public static bool isItemSelected = false;
	public static RawInventoryItem selectedItem;
	public static RawInventoryItem heldItem;
	const string droppedItemNodePath = "res://scenes/world/dropped_item.tscn";
	
	public bool isOpen = false;
    string slotNodePath = "res://scenes/ui/inventory_slot.tscn";
    Control _selectedItemNode;
    const string INVENTORY_GRID_GROUP = "InventoryGrid";
	const string INVENTORY_PANEL_GROUP = "InventoryPanel";
	
    const string HOTBAR_PANEL_GROUP = "HotbarPanel";
	const string HOTBAR_GRID_GROUP = "HotbarGrid";

	
	
	const int SELECTED_ITEM_OFFSET = 64;
	const int HOTBAR_SIZE = 8;
	static int _hotbarStartIndex = 24;


	public override void _Ready()
	{
		var tree = GetTree();
		
		_inventoryPanel = (Control) tree.GetFirstNodeInGroup(INVENTORY_PANEL_GROUP);
		_inventoryGrid = (Control) tree.GetFirstNodeInGroup(INVENTORY_GRID_GROUP);
		
		_hotbarPanel = (Control) tree.GetFirstNodeInGroup(HOTBAR_PANEL_GROUP);
		_hotbarGrid = (Control) tree.GetFirstNodeInGroup(HOTBAR_GRID_GROUP);
		
		_selectedItemNode = GetNode<Control>("SelectedItem");
		selectedIcon = GetNode<TextureRect>("SelectedItem/Icon");
		selectedQuantityLabel = GetNode<Label>("SelectedItem/Quantity");

		heldItemIndicator = GetNode<Sprite2D>("HeldItemIndicator");

		_inventoryPanel.Visible = false;
		_InitInventory();
	}


    public override void _Process(double delta) {
        _UpdateSelectedItem();
    }


	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("toggle_inventory")) {
			isOpen = !isOpen;
			_inventoryPanel.Visible = isOpen;
		}

		for (int hotbarKey = 0; hotbarKey < 8; hotbarKey ++) {
			int actualKeyNumber = hotbarKey + 1;
			if (@event.IsActionPressed("hotbar_" + actualKeyNumber.ToString())) {
				UpdateHeldItem(hotbarKey);
				return;
			}
		}

	}


	void UpdateHeldItem(int hotbarIndex) {
		int itemInventoryIndex =_hotbarStartIndex + hotbarIndex;
		heldItem = SaveData.organizedPlayerInventory[itemInventoryIndex];

		Vector2 heldItemPos = _hotbarGrid.GetChild<Control>(hotbarIndex).GlobalPosition;
		heldItemPos.X += SELECTED_ITEM_OFFSET / 2;
		heldItemPos.Y += SELECTED_ITEM_OFFSET / 2;

		heldItemIndicator.GlobalPosition = heldItemPos;
	}


	async Task _InitInventory() {
		SaveData.organizedPlayerInventory.Clear();

		foreach (var node in _inventoryGrid.GetChildren())
		{
			node.QueueFree();
		}
		
		foreach (var node in _hotbarGrid.GetChildren())
		{
			node.QueueFree();
		}

		_hotbarStartIndex = PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE - HOTBAR_SIZE;

		// Init all slots with null items
		for (int i = 0; i < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE; i++) {
			
			var itemSlotNode = GD.Load<PackedScene>(slotNodePath);
			var itemSlot = itemSlotNode.Instantiate<Control>();

			// This should probably be done better, works for now
			if (IsSlotInHotbar(i)) {
				_hotbarGrid.AddChild(itemSlot);
			}
			else {
				_inventoryGrid.AddChild(itemSlot);
			}
            
			((InventorySlot)itemSlot).UpdateSlot(null, i, false);
		}
		
		await PlayerInventoryData.ReadInventoryDataFromFile(SaveData.LoadData());
        
		for (int i = 0; i < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE; i++)
		{

			int slotIdx = (SaveData.organizedPlayerInventory[i] != null)
				? SaveData.organizedPlayerInventory[i].indexInOrganizedInventory
				: -1;

			if (slotIdx < 0 || slotIdx > PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE - 1) slotIdx = i;
			
			if (IsSlotInHotbar(i)) {
				_hotbarGrid.GetChild<InventorySlot>(i - _hotbarStartIndex)
					.UpdateSlot(SaveData.organizedPlayerInventory[i], slotIdx, false);
			}
			else {
				_inventoryGrid.GetChild<InventorySlot>(i)
					.UpdateSlot(SaveData.organizedPlayerInventory[i], slotIdx, false);
			}
        }
		
		UpdateHeldItem(0);
		SaveData.SyncInventory();
	}


	static bool IsSlotInHotbar(int index) {
		return index >= _hotbarStartIndex;
	}


    void _UpdateSelectedItem() {
		Vector2 mousePosition = GetGlobalMousePosition();
		mousePosition.X -= SELECTED_ITEM_OFFSET;
		mousePosition.Y -= SELECTED_ITEM_OFFSET;

		_selectedItemNode.GlobalPosition = mousePosition;
		_selectedItemNode.Visible = isItemSelected;
	}


	public static void SelectItem(RawInventoryItem rawItem) {
		
		if (rawItem == null) {
			GD.Print("Null item selected");
			return;
		}

		isItemSelected = true;
		selectedItem = rawItem;

		
		Item itemResource = ItemData.GetItemById(selectedItem.id);

		Texture2D iconTexture = itemResource.IconTexture;
		selectedIcon.Texture = iconTexture;

		selectedQuantityLabel.Visible = selectedItem.quantity > 1;
		selectedQuantityLabel.Text = selectedItem.quantity.ToString();
	}


	/// <summary> Main inventory additive operation </summary>
	/// <param name="rawItem"> Includes id, name and quantity to add </param>
	/// <param name="index"> optional: desired index in the organized player inventory array </param>
	/// <param name="affectSelectedItem"> optional : affect the current selected item </param>
	/// <param name="deselectOnAllAdded"> optional : deselect current item if all items were added </param>
	/// 
	///	<returns> how many items could *NOT* be added </returns>
	public static int AddItem(RawInventoryItem rawItem, int index = -1, bool affectSelectedItem = false, bool deselectOnAllAdded = true)
	{

		if (rawItem == null)
		{
			GD.PrintErr("Tried to add a null item to inventory @PlayerInventoryController.AddItem!");
			return -1;
		}
		
		rawItem.quantity = index == -1 
			? AddToInventoryUntilFull(rawItem) // index not specified
			: AddToSlotUntilFull(rawItem, index); // index given

		if (rawItem.quantity == 0 && rawItem.isValidIndex)
		{
			NullifyInventoryItemAtIndex(rawItem.indexInOrganizedInventory);
		}
		
        if (affectSelectedItem && isItemSelected)
        {
	        if (deselectOnAllAdded && rawItem.quantity == 0)
	        {
		        DeselectItem();
	        }
	        else if (selectedItem.id == rawItem.id && selectedItem.isValidIndex)
	        {
		        SelectItemAtSlot(selectedItem.indexInOrganizedInventory);
	        }
				    
        }
        
        SaveData.SyncInventory();
        return rawItem.quantity;
	}

	static int AddToInventoryUntilFull(RawInventoryItem itemToAdd)
	{
		for (int i = 0; i < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE; i++)
		{
			RawInventoryItem rawItem = null;
			
			if (SaveData.organizedPlayerInventory.Count > i)
			{
				rawItem = SaveData.organizedPlayerInventory[i];
			}

			if (rawItem == null || itemToAdd.id == rawItem.id)
			{
				itemToAdd.quantity = AddToSlotUntilFull(itemToAdd, i);
			}

			if (itemToAdd.quantity == 0) break;
		}
		
		return itemToAdd.quantity;
	}
	
	static int AddToSlotUntilFull(RawInventoryItem itemToAdd, int index)
	{
        
		RawInventoryItem itemInSlot = (SaveData.organizedPlayerInventory.Count > index)
			? SaveData.organizedPlayerInventory[index]
			: null;

		int spaceRemainingAtIndex = itemInSlot != null
			? itemInSlot.SpaceRemainingInStack
			: itemToAdd.stackSize;
		
		int amountToAdd = itemToAdd.quantity;

		int howManyWereAdded = (spaceRemainingAtIndex - amountToAdd < 0)
			? spaceRemainingAtIndex
			: amountToAdd;

		if (howManyWereAdded > 0)
		{
			RawInventoryItem addedItem = new RawInventoryItem(
				itemToAdd.id,
				itemToAdd.name,
				itemToAdd.stackSize - spaceRemainingAtIndex + howManyWereAdded,
				itemToAdd.stackSize);
			
            UpdateInventorySlot(addedItem, index);

			itemToAdd.quantity -= howManyWereAdded;
		}
        
		return itemToAdd.quantity;
	}
	

	/// <summary> Main inventory item removal method </summary>
	/// 
	/// <param name="rawItem"> Includes id, name and quantity to remove </param>
	///	<returns> Whether the add operation was successful or not </returns>
	/// <remarks> Automatically detects correct slots to remove items from </remarks>
	public static bool RemoveItemFromInventory(RawInventoryItem rawItem)
	{
		if (!PlayerInventoryData.ExistsInInventory(rawItem.id, rawItem.quantity))
		{
			return false;
		}

		int amountToRemove = rawItem.quantity;
		
		for (int i = PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE - 1; i >= 0; i--)
		{
			if (SaveData.organizedPlayerInventory[i] == null) continue;
			
			if (SaveData.organizedPlayerInventory[i].id == rawItem.id)
			{
				RawInventoryItem itemInSlot = SaveData.organizedPlayerInventory[i];
				
				int amountRemoved = (itemInSlot.quantity - amountToRemove > 0)
					? amountToRemove
					: itemInSlot.quantity;

				amountToRemove -= amountRemoved;
				itemInSlot.quantity -= amountRemoved;
				
				if (itemInSlot.quantity > 0)
				{
					UpdateInventorySlot(itemInSlot, i);
				}
				else
				{
					NullifyInventoryItemAtIndex(i);
				}

				if (isItemSelected && selectedItem.id == rawItem.id)
				{
					if (PlayerInventoryData.ExistsInInventory(selectedItem.id, 1))
					{
						SelectItemAtSlot(itemInSlot.quantity > 0
							? selectedItem.indexInOrganizedInventory
							: PlayerInventoryData.GetFirstStackIndexOfItem(selectedItem.id));
					}
					else
					{
						DeselectItem();
					}
				}
			}
			
			if (amountToRemove == 0) break;
        }

		switch (amountToRemove)
		{
			case 0:
				SaveData.SyncInventory();
				return true;
				
			case > 0:
				GD.PrintErr("Didn't remove enough items from inventory! @PlayerInventoryController.cs");
				return false;
				
			case < 0:
				GD.PrintErr("Removed too many items from inventory! @PlayerInventoryController.cs");
				return false;
		}
	}
	
	
    static void NullifyInventoryItemAtIndex(int index)
	{
		UpdateInventorySlot(null, index);
	}


	public static void SelectSingleItem(RawInventoryItem item, int index)
	{
		item.quantity -= 1;

		if (item.quantity >= 1) {
			UpdateInventorySlot(item, index);
		}
		else {
			NullifyInventoryItemAtIndex(index);
		}

		
		SelectItem(new RawInventoryItem(item.id, item.name, 1, item.stackSize));
	}

    static void SelectItemAtSlot(int index)
	{
		InventorySlot slot = IsSlotInHotbar(index) 
			? _hotbarGrid.GetChild<InventorySlot>(index - _hotbarStartIndex)
			: _inventoryGrid.GetChild<InventorySlot>(index);
		
		SelectItem(slot.slotItem);
		slot.ToggleVisuals(false);
	}

	public static void SwapItems(RawInventoryItem slotItem, int index)
	{
		RawInventoryItem tempSwapItem = new(
			slotItem.id, slotItem.name, slotItem.quantity, slotItem.stackSize);
        
		UpdateInventorySlot(selectedItem, index);

		UpdateInventorySlot(tempSwapItem, selectedItem.indexInOrganizedInventory);
		
		SelectItemAtSlot(selectedItem.indexInOrganizedInventory);
	}

	static void UpdateInventorySlot(RawInventoryItem item, int index)
	{
		InventorySlot slotToUpdate;
		if (IsSlotInHotbar(index)) {
			slotToUpdate = _hotbarGrid.GetChild<InventorySlot>(index - _hotbarStartIndex);
		}
		else {
			slotToUpdate = _inventoryGrid.GetChild<InventorySlot>(index);
		}

		slotToUpdate.UpdateSlot(item, index);
	}

	public static void DropSelectedItem(Vector2 position, Node parent)
	{
		RawInventoryItem temp = new RawInventoryItem(
			selectedItem.id, selectedItem.name, selectedItem.quantity, selectedItem.stackSize);

		CreateDroppedItem(temp, position, parent);
		
		if (selectedItem.isValidIndex)
		{
			NullifyInventoryItemAtIndex(selectedItem.indexInOrganizedInventory);
		}
		
		DeselectItem();
		
		// Select a new item of the same type if exists
		int index = PlayerInventoryData.GetFirstStackIndexOfItem(temp.id);
		
		if (index == 0)
		{
			if (!PlayerInventoryData.ExistsInInventory(temp.id, temp.quantity + 1)) return;
		}
		
		SelectItemAtSlot(index);
	}

	/// <summary>
	/// Creates a dropped item on the ground at the specified position
	/// </summary>
	/// <param name="item"></param>
	/// <param name="atPosition"></param>
	/// <param name="toParent"> Object for which the item will be parented to</param>
	/// <returns> Reference to the created dropped item Node2D </returns>
	public static Node2D CreateDroppedItem(RawInventoryItem item, Vector2 atPosition, Node toParent)
	{
		var droppedItemNode = GD.Load<PackedScene>(droppedItemNodePath).Instantiate<Node2D>();

		droppedItemNode.GlobalPosition = atPosition;
		toParent.AddChild(droppedItemNode);
		
		DroppedItem script = droppedItemNode as DroppedItem;
		
		script?.SetItem(item);

		return droppedItemNode;
	}
	
	public static void DeselectItem()
	{
		selectedItem = null;
		isItemSelected = false;
	}
}
