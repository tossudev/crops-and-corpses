using Godot;
using System;
using System.Diagnostics;

public partial class CraftPanel : Control
{
	[Export] public Label itemLabel;
	[Export] public TextureRect itemImage;
    
	[Export] public InventorySlot[] requiredResSlots;
	
	[Export] public TextEdit AmountToCraftTextEdit;
	[Export] public Button craftButton;

	public Item currentItemToBeCrafted;
	
	
	
	
	// Called when the node enters the scene tree for the first time.
    public const string GROUP_NAME = "CraftPanel";
    public override void _Ready()
	{
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
		Visible = true;
		currentItemToBeCrafted = craftItem;

		itemLabel.Text = currentItemToBeCrafted.Name.ToUpper();
		itemImage.Texture = currentItemToBeCrafted.IconTexture;

		for (int i = 0; i < requiredResSlots.Length; i++)
		{
			if (i > currentItemToBeCrafted.craftingRequirements.Length - 1)
			{
				requiredResSlots[i].Visible = false;
				continue;
			}
			
			requiredResSlots[i].Visible = true;

			var requiredItem = currentItemToBeCrafted.craftingRequirements[i];
			
			requiredResSlots[i].icon.Texture = requiredItem.item.IconTexture;
			requiredResSlots[i].itemID = requiredItem.item.ID;
			requiredResSlots[i].quantity = requiredItem.quantity;
			requiredResSlots[i].quantityLabel.Text = requiredItem.quantity.ToString();
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
			GD.PrintErr("Not a valid number of items to craft");
		}
		
		TryCraft(Mathf.Max(1, amountToCraft));
	}

	bool TryCraft(int amount)
	{
		try
		{
			// Checking that all requirements are met
			for (int i = 0; i < requiredResSlots.Length; i++)
			{
				if (!requiredResSlots[i].Visible) continue;
				
				if (!PlayerInventoryData.ExistsInInventory(requiredResSlots[i].itemID, requiredResSlots[i].quantity))
				{
					return false;
				}
			}
			
			for (int i = 0; i < requiredResSlots.Length; i++)
			{
				if (!requiredResSlots[i].Visible) continue;

				PlayerInventoryData.RemoveItemFromInventory(requiredResSlots[i].itemID, requiredResSlots[i].quantity);
			}

			PlayerInventoryData.AddItemToInventory(currentItemToBeCrafted.ID, amount);
			return true;
		}
		catch (Exception e)
		{
			Debug.Fail(e.Message);

			return false;
		}
	}
}
