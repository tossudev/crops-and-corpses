using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public partial class CraftPanel : Control
{
	List<int> craftableAmounts = new List<int>()
	{
		1,
		5,
		10,
		25,
		100
	};

	int _amountToCraft;
	
	Label _itemLabel;
	const string CRAFT_ITEM_LABEL_NODENAME = "%CraftItemLabel";
	
	TextureRect _itemImage;
	const string CRAFT_ITEM_IMAGE_NODENAME = "%CraftItemImage";

	[Export] StorageSlot[] _requiredResSlots;

	Label _errorMsgLabel;
	const string ERROR_LABEL_NODENAME = "%ErrorLabel";

	
	OptionButton _amountToCraftDropdown;
	const string AMOUNT_DROPDOWN_NODENAME = "%CraftAmountDropdown";

	
	Button _craftButton;
	const string CRAFT_BUTTON_NODENAME = "%CraftButton";


	public Item craftedItem;
	public RawInventoryItem currentItemAsRaw;
	
	
	// Called when the node enters the scene tree for the first time.
    public const string GROUP_NAME = "CraftPanel";
    public override void _Ready()
    {
	    Visible = false;

	    _itemLabel = GetNode<Label>(CRAFT_ITEM_LABEL_NODENAME);
	    _itemImage = GetNode<TextureRect>(CRAFT_ITEM_IMAGE_NODENAME);
	    _errorMsgLabel = GetNode<Label>(ERROR_LABEL_NODENAME);
	    _craftButton = GetNode<Button>(CRAFT_BUTTON_NODENAME);
	    _craftButton.Pressed += OnCraftButtonPressed;
	    
	    _amountToCraftDropdown = GetNode<OptionButton>(AMOUNT_DROPDOWN_NODENAME);
	    foreach (var amount in craftableAmounts)
	    {
		    _amountToCraftDropdown.AddItem(amount.ToString());
	    }
	    _amountToCraftDropdown.ItemSelected += (id) => UpdateRequirements();

	    _amountToCraftDropdown.Select(0);

    }

	public void OpenPanel(Item craftItem)
	{
		_errorMsgLabel.Visible = false;
		
		Visible = true;
		craftedItem = craftItem;
		currentItemAsRaw = new RawInventoryItem(craftItem.ID, craftItem.Name, 0, craftItem.StackSize);

		_itemLabel.Text = craftedItem.Name.ToUpper();
		_itemImage.Texture = craftedItem.IconTexture;

		UpdateRequirements();
	}

	public void ClosePanel()
	{
		Visible = false;
	}

    async void OnCraftButtonPressed()
	{
        if (!await TryCraft(Mathf.Max(1, _amountToCraft)))
		{
			_errorMsgLabel.Visible = true;
			_errorMsgLabel.Text = "Not enough resources";
		}
		else
		{
			_errorMsgLabel.Visible = false;
		}
	}

    void UpdateRequirements()
    {
	    _amountToCraft = int.Parse(_amountToCraftDropdown.Text);
	    
	    for (int i = 0; i < _requiredResSlots.Length; i++)
	    {
		    if (i > craftedItem.craftingRequirements.Length - 1)
		    {
			    _requiredResSlots[i].Visible = false;
			    continue;
		    }
		    _requiredResSlots[i].InitializeSlot(-1);
		    _requiredResSlots[i].Visible = true;

		    CraftingRequirement requirement = craftedItem.craftingRequirements[i];



		    RawInventoryItem requiredAsRaw = new RawInventoryItem(
			    requirement.item.ID, requirement.item.Name, requirement.quantity * _amountToCraft, requirement.item.StackSize);
			
		    _requiredResSlots[i].icon.Texture = requirement.item.IconTexture;
		    _requiredResSlots[i].slotItem = requiredAsRaw;
		    _requiredResSlots[i].quantityLabel.Text = (requirement.quantity * _amountToCraft).ToString();
	    }
    }
    
	async Task<bool> TryCraft(int amountToCraft)
	{
		try
		{
			if (craftedItem.craftingRequirements.Any(
				    craftingRequirement => !StorageData.ExistsInStorage(
                    SaveData.organizedPlayerInventory,
				    craftingRequirement.item.ID,craftingRequirement.quantity * amountToCraft)))
			{
				return false;
			}
			
			foreach (var craftingRequirement in craftedItem.craftingRequirements)
			{
				craftingRequirement.quantity *= amountToCraft;


				if (!await PlayerInventoryController.RemoveItemFromInventory(craftingRequirement.RequirementAsRaw()))
				{
					return false;
				}
			}

			await PlayerInventoryController.AddItemToInventory(
				new RawInventoryItem(craftedItem.ID, craftedItem.Name, amountToCraft, craftedItem.StackSize));
			
			return true;
		}
		catch (Exception e)
		{
			GD.PrintErr(e.Message);
			return false;
		}
	}
}
