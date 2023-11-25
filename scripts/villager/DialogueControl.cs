using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static VillagerManager;

public  partial class DialogueControl : Control
{
	TextureRect _villagerHatTextureRect;
	const string VILLAGER_HAT_TEXTURE_NODENAME = "%VillagerHatTexture";

	TextureRect _villagerHeadTextureRect;
	const string VILLAGER_HEAD_TEXTURE_NODENAME = "%VillagerHeadTexture";

	Label _nameText;
	const string VILLAGER_NAME_NODENAME = "%VillagerName";

	RichTextLabel _loreText;
	const string VILLAGER_LORE_NODENAME = "%VillagerLore";

	Label _statusText;
	const string VILLAGER_STATUS_NODENAME = "%VillagerStatus";

	Label _moodText;
	const string MOOD_TEXT_NODENAME = "%MoodText";
	
	
	
	
	Button _builderButton;
	const string BUILDER_BUTTON_NODENAME = "%BuilderButton";
	
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

	List<Button> _allOccupationButtons = new();
    
    
	Villager _assignedVillager;

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
		
		_villagerHatTextureRect = GetNode<TextureRect>(VILLAGER_HAT_TEXTURE_NODENAME);
		_villagerHeadTextureRect = GetNode<TextureRect>(VILLAGER_HEAD_TEXTURE_NODENAME);
		_nameText = GetNode<Label>(VILLAGER_NAME_NODENAME);
		_loreText = GetNode<RichTextLabel>(VILLAGER_LORE_NODENAME);
		_statusText = GetNode<Label>(VILLAGER_STATUS_NODENAME);
		_moodText = GetNode<Label>(MOOD_TEXT_NODENAME);

		_builderButton = GetNode<Button>(BUILDER_BUTTON_NODENAME);
		_farmerButton = GetNode<Button>(FARMER_BUTTON_NODENAME);
		_soldierButton = GetNode<Button>(SOLDIER_BUTTON_NODENAME);
		_woodcutterButton = GetNode<Button>(WOODCUTTER_BUTTON_NODENAME);
		_minerButton = GetNode<Button>(MINER_BUTTON_NODENAME);
		
		_dismissButton = GetNode<Button>(DISMISS_BUTTON_NODENAME);

		_allOccupationButtons.AddRange(new []
		{
			_builderButton,
			_farmerButton,
			_soldierButton,
			_woodcutterButton,
			_minerButton,
		});
		
		_builderButton.Pressed += () => SetNewOccupation(VillagerOccupation.Builder);
		
		_farmerButton.Pressed += () => SetNewOccupation(VillagerOccupation.Farmer);
		
		_soldierButton.Pressed += () => SetNewOccupation(VillagerOccupation.Soldier);
		
		_woodcutterButton.Pressed += () => SetNewOccupation(VillagerOccupation.Woodcutter);
		
		_minerButton.Pressed += () => SetNewOccupation(VillagerOccupation.Miner);
		
		_dismissButton.Pressed += ExitDialogue;
		
		ExitDialogue();
	}

	void SetNewOccupation(VillagerOccupation occupation)
	{
		villagerManagerInstance.SetVillagerOccupation(_assignedVillager, occupation);
		ExitDialogue();
	}
	
    void AssignVillager(Villager villager)
	{
		_assignedVillager = villager;

		VillagerRawData data = villager.rawData;
        
		_villagerHatTextureRect.Texture = villager.villagerInfo.villagerHatTexture;
		_villagerHatTextureRect.Visible = data.currentOccupation != VillagerOccupation.Builder;
		
		_villagerHeadTextureRect.Texture = villager.villagerInfo.villagerHeadTexture;

		_nameText.Text = data.name;
		_loreText.Text = data.lore;
		UpdateStatusAndMood(data.currentOccupation, data.currentState);
		UpdateActiveOccupationButtons(data.currentOccupation);
	}

    void UpdateStatusAndMood(VillagerOccupation occupation, VillagerState state)
    {
	    // Status
	    _statusText.Text = "Status:  ";
	    
	    _statusText.Text += state switch
	    {
		    VillagerState.RoamAround => "Wandering around",
		    VillagerState.FollowPlayer => "Following",
		    VillagerState.FixFence => "Repairing",
		    VillagerState.FindArcherTower => "Defending",
		    VillagerState.FindShelter => "Finding cover",
		    VillagerState.InShelter => "Waiting for sunrise",
		    VillagerState.ChooseTask => "Making a life choice",
		    VillagerState.FarmingTask => "Farming",
		    VillagerState.FindWoodTask => "Cutting wood",
		    VillagerState.FindStoneTask => "Mining stone",
		    VillagerState.ResqueQuest => "Waiting to be rescued",
		    _ => "error"
	    };
        
	    _moodText.Text = state switch
	    {
		    VillagerState.RoamAround when occupation == VillagerOccupation.Builder => "Work, work.. Wish I had some!",
		    VillagerState.RoamAround when occupation == VillagerOccupation.Farmer => "Crops grow while I walk around...",
		    VillagerState.RoamAround when occupation == VillagerOccupation.Soldier => "Man, nothing to shoot!",
		    VillagerState.RoamAround when occupation == VillagerOccupation.Woodcutter => "It's hard to chop if I ain't seeing any trees..",
		    VillagerState.RoamAround when occupation == VillagerOccupation.Miner => "All rocks of mine have been mined I guess...",
		    
		    VillagerState.FollowPlayer => "I'm following you",
		    
		    VillagerState.FixFence => "Can't you see that I'm busy?!",
                
		    VillagerState.FindArcherTower => "To arms!",
		    
		    VillagerState.FindShelter => "Time to hide!",
		    
		    VillagerState.InShelter when occupation == VillagerOccupation.Builder => "I hope there's not too much work in the morning!",
		    VillagerState.InShelter when occupation == VillagerOccupation.Farmer => "All I want is to keep my home!",
		    VillagerState.InShelter when occupation == VillagerOccupation.Soldier => "I need a tower to man, you know?!",
		    VillagerState.InShelter when occupation == VillagerOccupation.Woodcutter => "*Snoring*",
		    VillagerState.InShelter when occupation == VillagerOccupation.Miner => "My foundations will hold!",
		    
		    VillagerState.ChooseTask => "Let me think...",
		    
		    VillagerState.FarmingTask => "Grow cropsie, grow...",
		    
		    VillagerState.FindWoodTask => "C'mere trees!",
		    
		    VillagerState.FindStoneTask => "My pickaxe hand is tingling",
		    
		    VillagerState.ResqueQuest => "I need help!",
		    _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
	    };
    }

    void UpdateActiveOccupationButtons(VillagerOccupation occupation)
    {
	    _allOccupationButtons.ForEach(button => button.Visible = true);
	    
	    switch (occupation)
	    {
		    case VillagerOccupation.Builder:
			    _builderButton.Visible = false;
			    break;
		    case VillagerOccupation.Farmer:
			    _farmerButton.Visible = false;
			    break;
		    case VillagerOccupation.Soldier:
			    _soldierButton.Visible = false;
			    break;
		    case VillagerOccupation.Woodcutter:
			    _woodcutterButton.Visible = false;
			    break;
		    case VillagerOccupation.Miner:
			    _minerButton.Visible = false;
			    break;
		    default:
			    throw new ArgumentOutOfRangeException(nameof(occupation), occupation, null);
	    }
    }
    
	public void OpenDialogueWindow(Villager villager)
	{
		AssignVillager(villager);
		Visible = true;
	}

	
	public void ExitDialogue()
	{
		Visible = false;
	}
}
