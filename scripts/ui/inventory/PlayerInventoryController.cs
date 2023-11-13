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
	public static bool isInitialized = false;
	public static RawInventoryItem selectedItem;
	public static RawInventoryItem heldItem;
	const string droppedItemNodePath = "res://scenes/world/dropped_item.tscn";
	
	public bool isOpen = false;
    string slotNodePath = "res://scenes/ui/inventory/inventory_slot.tscn";
    Control _selectedItemNode;
    const string INVENTORY_GRID_GROUP = "InventoryGrid";
	const string INVENTORY_PANEL_GROUP = "InventoryPanel";
	
    const string HOTBAR_PANEL_GROUP = "HotbarPanel";
	const string HOTBAR_GRID_GROUP = "HotbarGrid";

	
	
	const int SELECTED_ITEM_OFFSET = 64;
	const int HOTBAR_SIZE = 8;
    public const int HOTBAR_START_IDX = PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE - HOTBAR_SIZE;


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
        UpdateSelectedItem();
    }


	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("toggle_inventory")) {
			isOpen = !isOpen;
			_inventoryPanel.Visible = isOpen;
		}
		
		if (@event.IsActionPressed("close_inventory")) {
			isOpen = false;
			_inventoryPanel.Visible = false;
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
		int itemInventoryIndex =HOTBAR_START_IDX + hotbarIndex;
		heldItem = SaveData.organizedPlayerInventory[itemInventoryIndex];

		Vector2 heldItemPos = _hotbarGrid.GetChild<Control>(hotbarIndex).GlobalPosition;
		heldItemPos.X += SELECTED_ITEM_OFFSET / 2;
		heldItemPos.Y += SELECTED_ITEM_OFFSET / 2;

		heldItemIndicator.GlobalPosition = heldItemPos;
	}


    async void _InitInventory()
    {
	    await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete, 100);
	    
		foreach (var node in _inventoryGrid.GetChildren())
		{
			node.Free();
		}
		
		foreach (var node in _hotbarGrid.GetChildren())
		{
			node.Free();
		}
        
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

			var inventorySlot = (InventorySlot)itemSlot;
			
			inventorySlot?.InitiateSlot(i);
		}
        
		for (int i = 0; i < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE; i++)
		{
			if (IsSlotInHotbar(i))
			{
				await _hotbarGrid.GetChild<InventorySlot>(i - HOTBAR_START_IDX)
					.UpdateSlot(SaveData.organizedPlayerInventory[i], false);
			}
			else 
			{
				await _inventoryGrid.GetChild<InventorySlot>(i)
					.UpdateSlot(SaveData.organizedPlayerInventory[i], false);
			}
        }
		
		UpdateHeldItem(0);
		await SaveData.SyncInventory();
		isInitialized = true;
    }


	static bool IsSlotInHotbar(int index) {
		return index >= HOTBAR_START_IDX;
	}


    void UpdateSelectedItem() {
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
	public static async Task<int> AddItem(RawInventoryItem rawItem, int index = -1, bool affectSelectedItem = false, bool deselectOnAllAdded = true)
	{

		if (rawItem == null)
		{
			GD.PrintErr("Tried to add a null item to inventory @PlayerInventoryController.AddItem!");
			return -1;
		}
		
		rawItem.quantity = index == -1 
			? await AddToInventoryUntilFull(rawItem) // index not specified
			: await AddToSlotUntilFull(rawItem, index); // index given

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
        
        Task sync = SaveData.SyncInventory();
        return rawItem.quantity;
	}

	static async Task<int> AddToInventoryUntilFull(RawInventoryItem itemToAdd)
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
				itemToAdd.quantity = await AddToSlotUntilFull(itemToAdd, i);
			}

			if (itemToAdd.quantity == 0) break;
		}
		
		return itemToAdd.quantity;
	}
	
	static async Task<int> AddToSlotUntilFull(RawInventoryItem itemToAdd, int index)
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
			
            await UpdateInventorySlot(addedItem, index);

			itemToAdd.quantity -= howManyWereAdded;
		}
        
		return itemToAdd.quantity;
	}

	/// <summary> Main inventory item removal method </summary>
	/// 
	/// <param name="rawItem"> Includes id, name and quantity to remove. </param>
	/// <param name="index"> (Optional) If specified, only removes the quantity available in that index. </param>
	///	<returns> Whether the removal operation was successful or not </returns>
	public static async Task<bool> RemoveItemFromInventory(RawInventoryItem rawItem, int index = -1)
	{
		if (rawItem == null)
		{
			GD.PushError("Tried to remove null item from inventory");
			return false;
		}
		
		if (!PlayerInventoryData.ExistsInInventory(rawItem.id, rawItem.quantity))
		{
			return false;
		}

		if (index >= 0)
		{
			if (index <= PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE - 1)
			{
				return await RemoveFromSlotUntilEmpty(
					rawItem, rawItem.quantity, index, true) == 0;
			}
			
			GD.PrintErr("Index was greater than player inventory max size - 1");
			return false;
		}
		
		switch (await RemoveFromInventoryUntilEmptyOfItem(rawItem))
		{
			case 0:
				Task sync = SaveData.SyncInventory();
				return true;
				
			case > 0:
				GD.PrintErr("Didn't remove enough items from inventory! @PlayerInventoryController.cs");
				return false;
				
			case < 0:
				GD.PrintErr("Removed too many items from inventory! @PlayerInventoryController.cs");
				return false;
		}
	}
	
	/// <summary>
	/// Removes quantity of item from inventory as long as it is possible
	/// </summary>
	/// <param name="itemToRemove"></param>
	/// <returns> How many could not be removed </returns>
	static async Task<int> RemoveFromInventoryUntilEmptyOfItem(RawInventoryItem itemToRemove)
	{
		int amountToRemove = itemToRemove.quantity;
		
		for (int i = PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE - 1; i >= 0; i--)
		{
			if (SaveData.organizedPlayerInventory[i] == null) continue;
			
			if (SaveData.organizedPlayerInventory[i].id == itemToRemove.id)
			{
				amountToRemove = await RemoveFromSlotUntilEmpty(itemToRemove, amountToRemove, i);
			}
			
			if (amountToRemove == 0) break;
		}
		
		return amountToRemove;
	}

	/// <summary>
	/// Removes items from inventory slot until all are removed OR slot's item quantity reaches 0.
	/// </summary>
	/// <param name="itemToRemove"> includes  </param>
	/// <param name="index"> must be valid </param>
	/// <param name="mustRemoveAll"> (optional) will not remove anything if itemToRemove.quantity is greater than quantity in slot. </param>
	/// <returns> amount that couldn't be removed </returns>
	static async Task<int> RemoveFromSlotUntilEmpty(RawInventoryItem itemToRemove, int amountToRemove, int index, bool mustRemoveAll = false)
	{
		RawInventoryItem itemInSlot = SaveData.organizedPlayerInventory[index];
				
		int amountRemoved = (itemInSlot.quantity - amountToRemove > 0)
			? amountToRemove
			: itemInSlot.quantity;

		if (mustRemoveAll && amountToRemove != amountRemoved) return amountToRemove;
		
		itemInSlot.quantity -= amountRemoved;
        
		if (itemInSlot.quantity > 0)
		{
			await UpdateInventorySlot(itemInSlot, index);
		}
		else
		{
			NullifyInventoryItemAtIndex(index);
		}
        
		return amountToRemove - amountRemoved;
	}
	
	
    static void NullifyInventoryItemAtIndex(int index)
	{
		Task update = UpdateInventorySlot(null, index);
	}


	public static void SelectSingleItem(RawInventoryItem item, int index)
	{
		item.quantity -= 1;

		if (item.quantity >= 1) {
			Task update = UpdateInventorySlot(item, index);
		}
		else {
			NullifyInventoryItemAtIndex(index);
		}

		
		SelectItem(new RawInventoryItem(item.id, item.name, 1, item.stackSize));
	}

    static void SelectItemAtSlot(int index)
	{
		InventorySlot slot = IsSlotInHotbar(index) 
			? _hotbarGrid.GetChild<InventorySlot>(index - HOTBAR_START_IDX)
			: _inventoryGrid.GetChild<InventorySlot>(index);
		
		SelectItem(slot.slotItem);
		slot.ToggleVisuals(false);
	}

	public static void SwapItems(RawInventoryItem slotItem, int index)
	{
		RawInventoryItem tempSwapItem = new(
			slotItem.id, slotItem.name, slotItem.quantity, slotItem.stackSize);
        
		Task update = UpdateInventorySlot(selectedItem, index);

		Task update2 = UpdateInventorySlot(tempSwapItem, selectedItem.indexInOrganizedInventory);
		
		SelectItemAtSlot(selectedItem.indexInOrganizedInventory);
	}

	static async Task UpdateInventorySlot(RawInventoryItem item, int index)
	{
		InventorySlot slotToUpdate;
		if (IsSlotInHotbar(index)) {
			slotToUpdate = _hotbarGrid.GetChild<InventorySlot>(index - HOTBAR_START_IDX);
		}
		else {
			slotToUpdate = _inventoryGrid.GetChild<InventorySlot>(index);
		}

		await slotToUpdate.UpdateSlot(item);
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
