
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

	public bool slotHasItem { get; private set; }
	
	public bool slotInitialized { get; private set; }
	
	int _slotIndex;
	public int slotIndex => _slotIndex;


	FloatingButtonName _floatingNamePanel;
    
	public override void _Ready()
	{
		base._Ready();
		_itemsRawArray = slotType switch
		{
			StorageSlotType.PlayerInventory => SaveData.organizedPlayerInventory,
			StorageSlotType.Hotbar => SaveData.playerHotbarItems,
			StorageSlotType.TownStorage => SaveData.townStorageItems,
			_ => throw new ArgumentOutOfRangeException()
		};
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
		    case false when slotHasItem:
			    PlayerInventoryController.SelectNewItem(slotItem);
			    ToggleVisuals(false);
			    break;
		    
		    // Player deselected item from hand
		    case true when !slotHasItem:
			    AddItemToInventory();
			    break;
		    
		    // Player has item a slot with item
		    case true when slotHasItem:
		    {
			    var selectedItem = PlayerInventoryController.selectedItem;
			    
			    if (selectedItem.HasValidIndexInArray(_itemsRawArray) &&
			        slotIndex == selectedItem.indexInStorage)
			    {
				    StorageSlotController.UpdateSlot(this, _itemsRawArray, PlayerInventoryController.selectedItem);
				    PlayerInventoryController.DeselectItem();
			    }

			    else if (slotItem.id == selectedItem.id)
			    {
				    AddItemToInventory();
			    }

			    else {
				    PlayerInventoryController.SwapItems(slotItem, slotIndex);
			    }

			    break;
		    }
	    }
    }


    protected void ClickRight()
    {
	    // Player takes one item from stack
	    if (!PlayerInventoryController.isItemSelected && slotHasItem)
	    {
		    var parent = GetParent<GridContainer>();
            
		    StorageController.SelectSingleItem(parent, _itemsRawArray, slotItem, slotIndex);
	    }
    }

    async void AddItemToInventory()
    {
	    await PlayerInventoryController.AddItemToInventory(PlayerInventoryController.selectedItem, slotIndex, true);
    }
    
    public void ToggleVisuals(bool on)
    {
	    icon.Visible = on;
	    quantityLabel.Visible = on;
    }

    public void UpdateVisuals()
    {
	    if (slotItem == null) {
		    slotHasItem = false;

		    icon.Texture = null;
		    quantityLabel.Text = "";
		    _floatingNamePanel.UpdateName("");
		    return;
	    }

	    slotHasItem = true;
		
	    Item itemResource = ItemData.GetItemById(slotItem.id);

	    Texture2D iconTexture = itemResource.IconTexture;
	    icon.Texture = iconTexture;

	    quantityLabel.Visible = itemResource.StackSize > 1;
	    quantityLabel.Text = slotItem.quantity.ToString();
		
	    _floatingNamePanel.UpdateName(slotItem.name);
    }
}