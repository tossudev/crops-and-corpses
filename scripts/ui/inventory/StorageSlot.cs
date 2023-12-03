
using Godot;

public abstract partial class StorageSlot : Control
{
	[Export] public bool isCraftingSlot;
	
	public TextureRect icon;
	const string ICON_TEXTURE_NODENAME = "%ItemIcon";

	public Label quantityLabel;
	const string QUANTITY_LABEL_NODENAME = "%ItemQuantityLabel";
	
	public RawInventoryItem slotItem;

	public bool slotHasItem { get; private set; }
	
	public bool slotInitialized { get; private set; }
	
	int _slotIndex;
	public int slotIndex => _slotIndex;


	FloatingButtonName _floatingNamePanel;
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

    protected abstract void ClickLeft();


    protected abstract void ClickRight();


    public abstract void ToggleVisuals(bool on);

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