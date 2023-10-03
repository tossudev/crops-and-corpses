using Godot;
using System;
using System.Diagnostics;

public partial class CraftPanel : Node
{
	[Export] public Label itemLabel;
	[Export] public TextureRect itemImage;
    
	[Export] public InventorySlot[] requiredResSlots;
	
	[Export] public TextEdit amountToCraft;
	[Export] public Button craftButton;
	
	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void OpenPanel()
	{
		
	}
	
	public void TryCraft()
	{
		try
		{
			//craft
		}
		catch (Exception e)
		{
			Debug.Fail(e.Message);
		}
	}
}
