
using System;
using Godot;
using Godot.Collections;

public partial class StorageSlot : Control
{
	[Export] public bool isCraftingSlot;
	[Export] public StorageSlotType slotType;
	
	public TextureRect icon;
	const string ICON_TEXTURE_NODENAME = "%ItemIcon";

	public Label quantityLabel;
	const string QUANTITY_LABEL_NODENAME = "%ItemQuantityLabel";
	
	public RawInventoryItem slotItem;
	Array<RawInventoryItem> _itemsRawArray;
	public Array<RawInventoryItem> itemsRawArray => _itemsRawArray;
	
	GridContainer _parentContainer;
	public GridContainer parentContainer => _parentContainer;

	public bool hasItem { get; private set; }
	
	public bool slotInitialized { get; private set; }
	
	int _slotIndex;
	public int slotIndex => _slotIndex;


	FloatingButtonName _floatingNamePanel;
    
	public override void _Ready()
	{
		base._Ready();
		if (isCraftingSlot) return;

		_itemsRawArray = slotType switch
		{
			StorageSlotType.PlayerInventory => SaveData.organizedPlayerInventory,
			StorageSlotType.Hotbar => SaveData.playerHotbarItems,
			StorageSlotType.TownStorage => SaveData.townStorageItems,
			_ => throw new ArgumentOutOfRangeException()
		};
		
		_parentContainer = GetParent<GridContainer>();
	}
	
    void OnButtonGuiInput(InputEvent @event)
	{
		if (isCraftingSlot) return;

		if (@event is not InputEventMouseButton { Pressed: true } mouseEvent) return;
		
		
		if (mouseEvent.ButtonIndex == MouseButton.Left)
		{
			ClickLeft();
		}
		else if (mouseEvent.ButtonIndex == MouseButton.Right)
		{
			ClickRight();
		}
	}

    public void InitializeSlot(int index)
    {
	    icon = GetNode<TextureRect>(ICON_TEXTURE_NODENAME);
	    quantityLabel = GetNode<Label>(QUANTITY_LABEL_NODENAME);
	    _floatingNamePanel = GetNode<FloatingButtonName>(FloatingButtonName.FLOATING_NAME_NODENAME);
	    
	    _slotIndex = index;

	    slotInitialized = true;
    }

	void ClickLeft()
    {
	    switch (PlayerInventoryController.isItemSelected)
	    {
		    // Player selects new item
		    case false when hasItem:
			    PlayerInventoryController.SelectNewItem(slotItem);
			    ToggleVisuals(false);
			    break;
		    
		    // Player deselected item from hand
		    case true when !hasItem:
			    AddItemToProperStorage();
			    break;
		    
		    // Player has clicked a slot with item in hand
		    case true when hasItem:
		    {
			    var selectedItem = PlayerInventoryController.selectedItem;
			    
			    if (PlayerInventoryController.HasSameItemSelected(slotItem))
			    {
				    // Return item back to its place
				    StorageSlotController.UpdateSlot(this, _itemsRawArray, selectedItem);
				    PlayerInventoryController.DeselectItem();
			    }
				else if (selectedItem.id == slotItem.id)
			    {
				    AddSelectedItemToSlot();
			    }
			    else
			    {
				    PlayerInventoryController.SwapItems(slotItem, slotIndex);
			    }

			    break;
		    }
	    }
    }


    protected void ClickRight()
    {
	    // Player takes one item from stack
	    if (!PlayerInventoryController.isItemSelected && hasItem)
	    {
		    StorageController.SelectSingleItem(parentContainer, _itemsRawArray, slotItem, slotIndex);
	    }
    }

    async void AddSelectedItemToSlot()
    {
	    await StorageController.AddItem(parentContainer, _itemsRawArray, PlayerInventoryController.selectedItem, slotIndex);
    }

	void AddItemToProperStorage()
    {
	    switch (slotType)
	    {
		    case StorageSlotType.Uninitialized:
			    break;
		    case StorageSlotType.PlayerInventory:
			    AddItemToInventory();
			    break;
		    case StorageSlotType.Hotbar:
			    AddItemToHotbar();
			    break;
		    case StorageSlotType.TownStorage:
			    AddItemToTownStorage();
			    break;
		    default:
			    throw new ArgumentOutOfRangeException();
	    }
    }
    
    async void AddItemToInventory()
    {
	    await PlayerInventoryController.AddItemToInventory(PlayerInventoryController.selectedItem, slotIndex);
    }
    
    async void AddItemToHotbar()
    {
	    await PlayerInventoryController.AddItemToHotbar(PlayerInventoryController.selectedItem, slotIndex);
    }
    
    async void AddItemToTownStorage()
    {
	    await TownStorageController.AddItemToTownStorage(PlayerInventoryController.selectedItem, slotIndex);
    }
    
    public void ToggleVisuals(bool on)
    {
	    icon.Visible = on;
	    quantityLabel.Visible = on;
    }

    public void UpdateVisuals()
    {
	    if (slotItem == null) {
		    hasItem = false;

		    icon.Texture = null;
		    quantityLabel.Text = "";
		    _floatingNamePanel.UpdateName("");
		    return;
	    }

	    hasItem = true;
		
	    Item itemResource = ItemData.GetItemById(slotItem.id);

	    Texture2D iconTexture = itemResource.IconTexture;
	    icon.Texture = iconTexture;

	    quantityLabel.Visible = itemResource.StackSize > 1;
	    quantityLabel.Text = slotItem.quantity.ToString();
		
	    _floatingNamePanel.UpdateName(slotItem.name);
    }
}