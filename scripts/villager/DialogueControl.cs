using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public  partial class DialogueControl : Control
{
	VillagerInfo _info;
	const string VILLAGER_INFO_NODENAME = "%VillagerInfo";
	
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

	public static DialogueControl instance;

	public override void _Ready()
	{
		base._Ready();
		
		if (instance != null)
		{
			if (instance.IsNodeReady())
			{
				instance.QueueFree();
			}
		}

		instance = this;
		
		_info = GetNode<VillagerInfo>(VILLAGER_INFO_NODENAME);
		_farmerButton = GetNode<Button>(FARMER_BUTTON_NODENAME);
		_soldierButton = GetNode<Button>(SOLDIER_BUTTON_NODENAME);
		_woodcutterButton = GetNode<Button>(WOODCUTTER_BUTTON_NODENAME);
		_minerButton = GetNode<Button>(MINER_BUTTON_NODENAME);
		_dismissButton = GetNode<Button>(DISMISS_BUTTON_NODENAME);
		_moodText = GetNode<RichTextLabel>(MOOD_TEXT_NODENAME);

		
		_farmerButton.Pressed += () => _hostVillager.ChangeOccupation(VillagerOccupation.Farmer);
		_farmerButton.Pressed += ExitDialogue;
		
		_soldierButton.Pressed += () => _hostVillager.ChangeOccupation(VillagerOccupation.Soldier);
		_soldierButton.Pressed += ExitDialogue;
		
		_woodcutterButton.Pressed += () => _hostVillager.ChangeOccupation(VillagerOccupation.Woodcutter);
		_woodcutterButton.Pressed += ExitDialogue;
		
		_minerButton.Pressed += () => _hostVillager.ChangeOccupation(VillagerOccupation.Miner);
		_minerButton.Pressed += ExitDialogue;
		
		_dismissButton.Pressed += ExitDialogue;
		
		ExitDialogue();
	}

	public void AssignVillager(Villager villager)
	{
		_hostVillager = villager;
		_info.UpdateStatus(villager.GetVillagerStates());
	}

	public void OpenDialogueWindow()
	{
		Visible = true;
		_info.OpenInfo();

		if (_hostVillager.GetVillagerStates() == VillagerState.RoamAround)
		{
			_moodText.Text = "Hello, do you need help?";
		}

		if (_hostVillager.GetVillagerStates() != VillagerState.RoamAround)
		{
			_moodText.Text = "I'M BUSY";
		} 
	}

	
	public void ExitDialogue()
	{
		Visible = false;
		
		_info.CloseInfo();
	}
}
