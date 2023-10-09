using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class CraftPanel : Control
{
	[Export] public Label itemLabel;
	[Export] public TextureRect itemImage;
    
	[Export] public InventorySlot[] requiredResSlots;

	[Export] public Label ErrorMsgLabel;
	[Export] public TextEdit AmountToCraftTextEdit;
	[Export] public Button craftButton;

	public Item craftedItem;
	public RawInventoryItem currentItemAsRaw;
	
	
	// Called when the node enters the scene tree for the first time.
    public const string GROUP_NAME = "CraftPanel";
    public override void _Ready()
    {
	    Visible = false;
    }

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (@event is InputEventMouseButton or InputEventKey
			&& @event.IsPressed())
		{
			ClosePanel();
		}
	}

	public void OpenPanel(Item craftItem)
	{
		ErrorMsgLabel.Visible = false;
		
		Visible = true;
		craftedItem = craftItem;
		currentItemAsRaw = new RawInventoryItem(craftItem.ID, craftItem.Name, 0, craftItem.StackSize);

		itemLabel.Text = craftedItem.Name.ToUpper();
		itemImage.Texture = craftedItem.IconTexture;

		for (int i = 0; i < requiredResSlots.Length; i++)
		{
			if (i > craftedItem.craftingRequirements.Length - 1)
			{
				requiredResSlots[i].Visible = false;
				continue;
			}
			
			requiredResSlots[i].Visible = true;

			CraftingRequirement requirement = craftedItem.craftingRequirements[i];



			RawInventoryItem requiredAsRaw = new RawInventoryItem(
				requirement.item.ID, requirement.item.Name, requirement.quantity, requirement.item.StackSize);
			
			requiredResSlots[i].icon.Texture = requirement.item.IconTexture;
			requiredResSlots[i].slotItem = requiredAsRaw;
			requiredResSlots[i].quantityLabel.Text = requirement.quantity.ToString();
		}
	}

	public void ClosePanel()
	{
		Visible = false;
	}

	public void _on_craft_button_pressed()
	{
		int amountToCraft = 0;
		
		try
		{
			amountToCraft = int.Parse(AmountToCraftTextEdit.Text);
		}
		catch (Exception e)
		{
			GD.PrintErr("Not a valid number of items to craft", e.Message);
			
			ErrorMsgLabel.Visible = true;
			ErrorMsgLabel.Text = "Enter a valid number";
		}

		if (!TryCraft(Mathf.Max(1, amountToCraft)))
		{
			ErrorMsgLabel.Visible = true;
			ErrorMsgLabel.Text = "Not enough resources";
		}
		else
		{
			ErrorMsgLabel.Visible = false;
		}
	}

	bool TryCraft(int amountToCraft)
	{
		try
		{
			if (craftedItem.craftingRequirements.Any(
				    craftingRequirement => !PlayerInventoryData.ExistsInInventory(
				    craftingRequirement.item.ID,craftingRequirement.quantity * amountToCraft)))
			{
				return false;
			}
			
			foreach (var craftingRequirement in craftedItem.craftingRequirements)
			{
				craftingRequirement.quantity *= amountToCraft;


				if (!PlayerInventoryController.RemoveItemFromInventory(craftingRequirement.RequirementAsRaw()))
				{
					return false;
				}
			}

			PlayerInventoryController.AddItem(
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
