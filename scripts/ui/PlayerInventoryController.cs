using Godot;
using System.Threading.Tasks;

public partial class PlayerInventoryController : Control {

	public static TextureRect selectedIcon;
	public static Label selectedQuantityLabel;

	static Control _inventoryGrid;
	
	public static bool isItemSelected = false;
	public static RawInventoryItem selectedItem;
	
	public bool isOpen = false;
    string slotNodePath = "res://scenes/ui/inventory_slot.tscn";
    Control _selectedItemNode;
	public const string PIC_NODE_GROUP = "PlayerInventoryController";
	const int SELECTED_ITEM_OFFSET = 64;


	public override void _Ready() {
		_inventoryGrid = GetNode<Control>("InventoryGrid");
		_selectedItemNode = GetNode<Control>("SelectedItem");
		selectedIcon = GetNode<TextureRect>("SelectedItem/Icon");
		selectedQuantityLabel = GetNode<Label>("SelectedItem/Quantity");
		_InitInventory();
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


	async Task _InitInventory() {
		SaveData.organizedPlayerInventory.Clear();

		// Init all slots with null items
		for (int i = 0; i < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE; i++) {
			
			var itemSlotNode = GD.Load<PackedScene>(slotNodePath);
			var itemSlot = itemSlotNode.Instantiate<Control>();

			_inventoryGrid.AddChild(itemSlot);
            
			((InventorySlot)itemSlot).UpdateSlot(null, i);
		}
		
		await PlayerInventoryData.ReadInventoryDataFromFile(SaveData.LoadData());
        
		for (int i = 0; i < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE; i++) {
			_inventoryGrid.GetChild<InventorySlot>(i)
				.UpdateSlot(SaveData.organizedPlayerInventory[i], i);
        }
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
	///
	///	<returns> how many items could *NOT* be added </returns>
	public static int AddItem(RawInventoryItem rawItem, int index = -1)
	{

		if (rawItem == null)
		{
			GD.PrintErr("Tried to add a null item to inventory @PlayerInventoryController.AddItem!");
			return -1;
		}

		
		rawItem.quantity = index == -1 
			? AddToInventoryUntilFull(rawItem) // index not specified
			: AddToSlotUntilFull(rawItem, index); // index given

		
        SaveData.SyncInventory();
		

		if (rawItem.quantity == 0)
		{
			DeselectItem();
			return 0;
		}
        
		if (isItemSelected)
		{
			SelectItem(rawItem);
		}
		
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
			return false;

		int amountToRemove = rawItem.quantity;
		
		for (int i = PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE - 1; i >= 0; i--)
		{
			if (SaveData.organizedPlayerInventory[i] == null) continue;
			
			if (SaveData.organizedPlayerInventory[i].id == rawItem.id)
			{
				int itemQuantityInSlot = SaveData.organizedPlayerInventory[i].quantity;
				
				int amountRemoved = (itemQuantityInSlot - amountToRemove > 0)
					? amountToRemove
					: itemQuantityInSlot;

				amountToRemove -= amountRemoved;
				
				if (amountRemoved < itemQuantityInSlot)
				{
					SaveData.organizedPlayerInventory[i].quantity -= amountRemoved;
					UpdateInventorySlot(SaveData.organizedPlayerInventory[i], i);
				}
				else
				{
					NullifyInventoryItemAtIndex(i);
				}
			}

			
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
	
	
	public static void NullifyInventoryItemAtIndex(int index)
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


	public static void SwapItems(RawInventoryItem itemToSwap, int index)
	{
		RawInventoryItem tempSwapItem = new(
			itemToSwap.id, itemToSwap.name, itemToSwap.quantity, itemToSwap.stackSize);
		
		UpdateInventorySlot(selectedItem, index);
		SelectItem(tempSwapItem);
	}

	static void UpdateInventorySlot(RawInventoryItem item, int index)
	{
		InventorySlot slotToUpdate = _inventoryGrid.GetChild<InventorySlot>(index);
		slotToUpdate.UpdateSlot(item, index);
	}
	
	public static void DeselectItem()
	{
		selectedItem = null;
		isItemSelected = false;
	}
}
