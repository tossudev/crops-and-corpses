using Godot;
using System;
using static VillagerManager;


public enum VillagerOccupation
{
	Unemployed,
	Farmer,
	Soldier,
	Woodcutter,
	Miner
}

public partial class Villager : CharacterBody2D
{
	VillagerStates _state;
	//[Export] VillagerManager _villagerManager;
	Vector2 _targetPosition;
	Timer _timer;
	Timer _gatheringTimer;

	[Export] DialogueControl dialogueControl;
	[Export] NavigationAgent2D navMeshAgent;
	//[Export] NavigationRegion2D navRegionArea;
	Plant _currentPlant;
	Node2D _streetSign;
	Sprite2D _villagerSprite;
	int _plantIndex = 0;
	float _speed = 0;
	bool collision = false;
	bool _timerStopped = false;
	bool _taskStarted = false;
	int _resourceTaskCounter = 0;

	[Export] VillagerInfo _info;

	string _villagerName;
	string _villagerInfo;

    VillagerOccupation _currentOccupation;

    public VillagerResidence currentResidence;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_streetSign = GetParent().GetParent().GetNode<Node2D>("StreetSignSpot");
		_villagerSprite = GetNode<Sprite2D>("Sprite2D");
		_gatheringTimer = GetNode<Timer>("GatheringTimer");

		dialogueControl.AssignVillager(this);
		
		_timer = new Timer
		{
			WaitTime = 1f,
		};
		_timer.Timeout += State;
		AddChild(_timer);
		_timer.Start();

/* 		string _currentScene = GetTree().CurrentScene.Name;

		if (_currentScene != null && _currentScene == "Forest")
        {
			GD.Print("Following player");
            _state = VillagerStates.FollowPlayer;
        }
        else
        {
			GD.Print("Roaming around");
            _state = VillagerStates.RoamAround;
        } */

		_state = VillagerStates.RoamAround;

		_villagerName = instance.GetVillagerData().name;
		_villagerInfo = instance.GetVillagerData().info;
		_villagerSprite.Texture = instance.GetVillagerData().texture;
		
		_info.InitializeVillagerInfo(_villagerSprite.Texture, _villagerName, _villagerInfo, _state);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (!dialogueControl.Visible && GlobalPosition.DistanceTo(_targetPosition) > 5)
		{
			Movement(_targetPosition);
		}
		if(_taskStarted && _resourceTaskCounter == 5)
		{
			ResourceGatheringDone();
		}
	}

	public VillagerStates GetVillagerStates()
	{
		return _state;
	}

	public void EnterShelter()
	{
		_state = VillagerStates.InShelter;
		Visible = false;
	}

	public void ExitShelter()
	{
		_state = VillagerStates.ChooseTask;
		Visible = true;
	}
	
	void State()
	{
        if (!TimeManager.dayTime)
        {
	        switch (_currentOccupation)
	        {
		        case VillagerOccupation.Soldier:
			        _state = VillagerStates.FindArcherTower;
			        break;

		        default:
		        {
			        if (_state != VillagerStates.InShelter)
			        {
				        _state = VillagerStates.FindShelter;

			        }
			        break;
		        }
	        }

	        return;
        }

        // Food for thought?
        // if (dialogueControl.Visible)
        // {
	       //  _state = VillagerStates.Idle;
        // }
		
        switch (_state)
		{
			case VillagerStates.RoamAround:
				RoamAround();
				break;

			case VillagerStates.FollowPlayer:
				FollowPlayer();
				break;

			case VillagerStates.ChooseTask:
				ChooseTask();
				break;

			case VillagerStates.FarmingTask:
				CheckPlants();
				break;

			case VillagerStates.FindWoodTask:
				GatherResources();
				break;

			case VillagerStates.FindStoneTask:
				GatherResources();
				break;
			
			case VillagerStates.GetHospitalized:
				//TODO
				break;

			case VillagerStates.FixFence:
				//TODO
				break;

			case VillagerStates.FindArcherTower:
				//TODO
				break;

			case VillagerStates.FindShelter:

				if (TimeManager.dayTime)
				{
					_state = VillagerStates.ChooseTask;
				}
				
				break;
			
			case VillagerStates.InShelter when TimeManager.dayTime:
				currentResidence.VillagerExitBuilding(this);
				break;

			default: 
				GD.Print("State not found");
				break;
		}
		//GD.Print(_state);
	}

	public void _on_button_button_up()
	{
		
		_info.Visible = true;
		_info.UpdateStatus(_state);
		dialogueControl.OpenDialogueWindow();
	}

	const string VILLAGER_RESIDENCE_NODENAME = "%TownHallMenu";
	public void _on_area_2d_area_entered(Area2D area)
	{
		collision = true;

		if (area.Owner.HasNode(VILLAGER_RESIDENCE_NODENAME))
		{
			currentResidence = area.Owner.GetNode<TownHallMenu>(VILLAGER_RESIDENCE_NODENAME)._villagerResidence;

			currentResidence?.VillagerEnterBuilding(this);
		}
	}

	void Movement(Vector2 target)
	{
		_speed = 100;
		Vector2 _direction = (target - GlobalPosition).Normalized();
		Velocity = _direction * _speed;
		MoveAndSlide();
	}

	void RoamAround()
	{
		float range = 100;
        _targetPosition = GlobalPosition + new Vector2(GD.Randf() * range * 8 - range, GD.Randf() * range * 8 - range);
	}
    
	void ChooseTask()
	{
		switch (_currentOccupation)
		{
			case VillagerOccupation.Farmer:
				GD.Print("Farming task started");
				_state = VillagerStates.FarmingTask;
				break;
			
			case VillagerOccupation.Soldier:
				break;
			
			case VillagerOccupation.Woodcutter:
				GD.Print("Finding wood");
				_state = VillagerStates.FindWoodTask;
				break;
			
			case VillagerOccupation.Miner:
				GD.Print("Finding stone");
				_state = VillagerStates.FindStoneTask;
				break;
			
			default:
				GD.Print("Villager Unemployed :(");
				_state = VillagerStates.RoamAround;
				break;
		}

		dialogueControl.ExitDialogue();
	}
	
	public void ChangeOccupation(VillagerOccupation occupation)
	{
		_currentOccupation = occupation;
	}
	
 	void GatherResources()
	{
		_targetPosition = _streetSign.GlobalPosition;
		if (GlobalPosition.DistanceTo(_targetPosition) < 5)
		{
			_info.Visible = false;
			_taskStarted = true;
			_villagerSprite.Visible = false;
			_resourceTaskCounter ++;
		}
	}

	void ResourceGatheringDone()
	{
		Random rnd = new Random();
		_villagerSprite.Visible = true;
		if(_currentOccupation == VillagerOccupation.Miner)
		{
			int amount = rnd.Next(4, 21);
			GD.Print("Found: " + amount + " stone");
		}
		
		if(_currentOccupation == VillagerOccupation.Woodcutter)
		{
			int amount = rnd.Next(4, 21);
			GD.Print("Found: " + amount + " wood");
		}
		
		_taskStarted = false;
		_resourceTaskCounter = 0;
		_state = VillagerStates.RoamAround;
	}

	void CheckPlants()
	{
		_currentPlant = FarmManager.instance.GetPlantedPlants()[_plantIndex];

		if (_currentPlant.GetGrowthState() == GrowthState.IsWilting || _currentPlant.GetGrowthState() == GrowthState.WaitWatering ||
			_currentPlant.GetGrowthState() == GrowthState.IsInfested)
		{
			_targetPosition = _currentPlant.GlobalPosition;
		}
		else
		{
			bool _allHarvestable = true;
			
			for (int i = 0; i < FarmManager.instance.GetPlantedPlants().Count; i++)
			{	
				if (FarmManager.instance.GetPlantedPlants()[i].GetGrowthState() != GrowthState.IsHarvestable &&
					FarmManager.instance.GetPlantedPlants()[i].GetGrowthState() != GrowthState.IsDead)
				{	
					_allHarvestable = false;
					break;
				}
			}
			if (_allHarvestable == false)
			{
				_plantIndex++;
				if (_plantIndex >= FarmManager.instance.GetPlantedPlants().Count)
				{
					_plantIndex = 0;
				}
				return;
			}
			else
			{
				_state = VillagerStates.RoamAround;
				return;
			}
		}
		if (GlobalPosition.DistanceTo(_currentPlant.GlobalPosition) < 5)
		{
			GD.Print("I am at the plant yay");
			if (_currentPlant.GetGrowthState() == GrowthState.IsWilting || _currentPlant.GetGrowthState() == GrowthState.WaitWatering)
			{
				_currentPlant.WaterPlant();
			}
			if (_currentPlant.GetGrowthState() == GrowthState.IsInfested)
			{
				_currentPlant.CurePlant();
			}
			if (_currentPlant.GetGrowthState() != GrowthState.IsWilting && _plantIndex < FarmManager.instance.GetPlantedPlants().Count ||
			 _currentPlant.GetGrowthState() != GrowthState.WaitWatering && _plantIndex < FarmManager.instance.GetPlantedPlants().Count ||
			_currentPlant.GetGrowthState() != GrowthState.IsInfested && _plantIndex < FarmManager.instance.GetPlantedPlants().Count)
			{
				_plantIndex++;
			}
			if (_plantIndex == FarmManager.instance.GetPlantedPlants().Count)
			{
				_plantIndex = 0;
			}
		}
	}

	void FollowPlayer()
	{
		//not working anymore(yet)
		_targetPosition = GetParent().GetNode<CharacterBody2D>("Forest/Objects/Player").GlobalPosition;
	}
}


