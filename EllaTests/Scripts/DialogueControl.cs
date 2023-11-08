using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public  partial class DialogueControl : Control
{
	Button _farmerButton;
	const string FARMER_BUTTON_NODENAME = "%FarmerButton";
	
	Button _soldierButton;
	const string SOLDIER_BUTTON_NODENAME = "%SoldierButton";
	
	Button _woodcutterButton;
	const string WOODCUTTER_BUTTON_NODENAME = "%WoodcutterButton";
	
	Button _minerButton;
	const string MINER_BUTTON_NODENAME = "%MinerButton";
	
	Button _dismissButton;
	const string DISMISS_BUTTON_NODENAME = "%DismissButton";
	
    RichTextLabel _moodText;
    const string MOOD_TEXT_NODENAME = "%MoodText";
    
	Villager _hostVillager;
	
	public void AssignVillager(Villager villager)
	{
		_hostVillager = villager;
        
		_farmerButton = GetNode<Button>(FARMER_BUTTON_NODENAME);
		_farmerButton.Pressed += () => _hostVillager.ChangeOccupation(VillagerOccupation.Farmer);
		
		_soldierButton = GetNode<Button>(SOLDIER_BUTTON_NODENAME);
		_soldierButton.Pressed += () => _hostVillager.ChangeOccupation(VillagerOccupation.Soldier);
		
		_woodcutterButton = GetNode<Button>(WOODCUTTER_BUTTON_NODENAME);
		_woodcutterButton.Pressed += () => _hostVillager.ChangeOccupation(VillagerOccupation.Woodcutter);
		
		_minerButton = GetNode<Button>(MINER_BUTTON_NODENAME);
		_minerButton.Pressed += () => _hostVillager.ChangeOccupation(VillagerOccupation.Miner);
		
		_dismissButton = GetNode<Button>(DISMISS_BUTTON_NODENAME);
		_dismissButton.Pressed += ExitDialogue;
        
		_moodText = GetNode<RichTextLabel>(MOOD_TEXT_NODENAME);
		
		Visible = false;
	}

	public void OpenDialogueWindow()
	{
		Visible = true;

		if (_hostVillager.GetVillagerStates() == VillagerManager.VillagerStates.RoamAround)
		{
			_moodText.Text = "Hello, do you need help?";
		}

		if (_hostVillager.GetVillagerStates() != VillagerManager.VillagerStates.RoamAround)
		{
			_moodText.Text = "I'M BUSY";
		} 
	}

	
	public void ExitDialogue()
	{
		Visible = false;
	}
}
