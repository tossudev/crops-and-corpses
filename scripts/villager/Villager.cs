using Godot;
using System;
using System.Collections.Generic;


public enum VillagerOccupation
{
	Builder,
	Farmer,
	Soldier,
	Woodcutter,
	Miner
}

public partial class Villager : CharacterBody2D
{
	VillagerState _state;
	//[Export] VillagerManager _villagerManager;
	Vector2 _targetPosition;
	Timer _timer;
	Timer _gatheringTimer;

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
	const string PLAYER_NODENAME = "%Player";
	const string STREETSIGN_NODENAME = "%StreetSign";
	const string FOREST_SCENE_NODENAME = "%ForestScene";
	CharacterBody2D _player;
	public bool needResque = false;

	[Export] VillagerInfo _info;
	public VillagerInfo villagerInfo => _info;

	VillagerRawData _rawData;
	public VillagerRawData rawData => _rawData;

	bool _inTownScene;

    VillagerOccupation _currentOccupation;
    public VillagerOccupation currentOccupation => _currentOccupation;

    public List<Villager> currentOccupationList;

    public VillagerResidence currentResidence;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetParent().GetNodeOrNull<CharacterBody2D>(PLAYER_NODENAME);
		_streetSign = GetParent().GetNodeOrNull<Node2D>(STREETSIGN_NODENAME);

		_villagerSprite = GetNode<Sprite2D>("Sprite2D");
		_gatheringTimer = GetNode<Timer>("GatheringTimer");
        
		_timer = new Timer
		{
			WaitTime = GD.RandRange(0.8f, 1.25f),
		};
		_timer.Timeout += State;
		AddChild(_timer);
		_timer.Start();

		_inTownScene = SceneManager.IsCurrentScene(this, Scene.Town);
		
		if(!_inTownScene)
		{
			needResque = true;
			_state = VillagerState.ResqueQuest;
		}
		else
		{
			needResque = false;
			_state = VillagerState.RoamAround;
		}
	}

	public void InitializeVillager(VillagerRawData data)
	{
		_rawData = data;
		
		ChangeOccupation(data.currentOccupation);
		_state = VillagerState.RoamAround;
        
		// TODO: This needs to be changed once the rig sprites are in place and saved
		_villagerSprite.Texture = VillagerManager.instance.GetNewVillagerData().texture;
		
		_info.InitializeVillagerInfo(_villagerSprite.Texture, data.name, data.lore, _state);
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.DistanceTo(_targetPosition) > 5)
		{
			Movement(_targetPosition);
		}
		if(_taskStarted && _resourceTaskCounter == 5)
		{
			ResourceGatheringDone();
		}
	}

	public VillagerState GetVillagerStates()
	{
		return _state;
	}

	public void EnterShelter()
	{
		_state = VillagerState.InShelter;
		_targetPosition = GlobalPosition;
		Visible = false;
	}

	public void ExitShelter()
	{
		_state = VillagerState.ChooseTask;
		Visible = true;
	}
	
	void State()
	{
        if (!TimeManager.dayTime && !_inTownScene)
        {
	        switch (_currentOccupation)
	        {
		        case VillagerOccupation.Soldier:
			        _state = VillagerState.FindArcherTower;
			        break;

		        default:
		        {
			        if (_state != VillagerState.InShelter)
			        {
				        _state = VillagerState.FindShelter;
			        }
			        break;
		        }
	        }
        }
		
        switch (_state)
		{
			case VillagerState.RoamAround:
				RoamAround();
				break;

			case VillagerState.FollowPlayer:
				FollowPlayer();
				break;

			case VillagerState.ChooseTask:
				ChooseTask();
				break;

			case VillagerState.FarmingTask:
				CheckPlants();
				break;

			case VillagerState.FindWoodTask:
				GatherResources();
				break;

			case VillagerState.FindStoneTask:
				GatherResources();
				break;

			case VillagerState.ResqueQuest:
				WaitingResque();
				break;

			case VillagerState.FixFence:
				//TODO
				break;

			case VillagerState.FindArcherTower:
				//TODO
				break;

			case VillagerState.FindShelter:

				if (TimeManager.dayTime)
				{
					_state = VillagerState.ChooseTask;
				}
				else
				{
					FindShelter();
				}
				
				break;
			
			case VillagerState.InShelter when TimeManager.dayTime:
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
		if(!_inTownScene && needResque)
		{
			OpenResqueDialogue();
		}
		else
		{
			OpenDialogue();
		}	
	}

	public void OpenDialogue()
	{
		DialogueControl.instance.AssignVillager(this);
		DialogueControl.instance.OpenDialogueWindow();
	}

	public void OpenResqueDialogue()
	{
		GD.Print("You saved me");
		// Tähä joku button tai joku ?????
		_state = VillagerState.FollowPlayer;
	}

	const string VILLAGER_RESIDENCE_NODENAME = "%TownHallMenu";
	public void _on_area_2d_area_entered(Area2D area)
	{
		collision = true;

		if (area.Owner.HasNode(VILLAGER_RESIDENCE_NODENAME))
		{
			currentResidence = area.Owner.GetNodeOrNull<TownHallMenu>(VILLAGER_RESIDENCE_NODENAME)._villagerResidence;

			currentResidence?.VillagerEnterBuilding(this);

			area.GetInstanceId();
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
		_targetPosition = GlobalPosition + CreateOffsetVector2(-200, 200);
	}

	void FindShelter()
	{
		_targetPosition = TownManager.townHallPosition - GlobalPosition + CreateOffsetVector2(-100, 100);
	}

	Vector2 CreateOffsetVector2(double min, double max)
	{
		GD.Randomize();
		float x = (float) GD.RandRange(min, max);
			
		GD.Randomize();
		float y = (float) GD.RandRange(min, max);
		
		return new Vector2(x, y);
	}
	
	void ChooseTask()
	{
		switch (_currentOccupation)
		{
			case VillagerOccupation.Farmer:
				GD.Print("Farming task started");
				_state = VillagerState.FarmingTask;
				break;
			
			case VillagerOccupation.Soldier:
				break;
			
			case VillagerOccupation.Woodcutter:
				GD.Print("Finding wood");
				_state = VillagerState.FindWoodTask;
				break;
			
			case VillagerOccupation.Miner:
				GD.Print("Finding stone");
				_state = VillagerState.FindStoneTask;
				break;
			
			default:
				_state = VillagerState.RoamAround;
				break;
		}

		DialogueControl.instance.ExitDialogue();
	}
	
	public void ChangeOccupation(VillagerOccupation occupation)
	{
		VillagerManager.instance.SetVillagerOccupation(this, occupation);
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
		_state = VillagerState.RoamAround;
	}

	void WaitingResque()
	{
		_targetPosition = GlobalPosition + CreateOffsetVector2(-10, 10);
	}

	void CheckPlants()
	{
		_currentPlant = FarmManager.instance.GetPlantedPlants()[_plantIndex];
		_currentPlant.isTendedTo = true;

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
				_state = VillagerState.RoamAround;
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
		_speed = 200;
		_targetPosition = _player.GlobalPosition;
	}
}


