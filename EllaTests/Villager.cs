using Godot;
using System;
using static VillagerManager;

public partial class Villager : CharacterBody2D
{
	VillagerStates _state;
	//[Export] VillagerManager _villagerManager;
	Vector2 _targetPosition;
	Timer _timer;
	Timer _gatheringTimer;

	[Export] DialogueControl dialogueControl;
	[Export] NavigationAgent2D navMeshAgent;
	[Export]TimeManager dayTimeCheck;
	//[Export] NavigationRegion2D navRegionArea;
	Plant _currentPlant;
	Node2D _streetSign;
	Sprite2D _villagerSprite;
	int _plantIndex = 0;
	float _speed = 0;
	bool collision = false;
	bool _timerStopped = false;
	bool _taskStarted = false;
	bool _dayTime;
	int _resourcheTaskCounter = 0;
	public bool dialogueWindow = false;

	[Export] Villager_Info _info;

	string _villagerName;
	string _villagerInfo;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_streetSign = GetParent().GetParent().GetNode<Node2D>("StreetSignSpot");
		_villagerSprite = GetNode<Sprite2D>("Sprite2D");
		_gatheringTimer = GetNode<Timer>("GatheringTimer");

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
		_dayTime = dayTimeCheck.dayTime;
		if(!_dayTime)
		{
			_state = VillagerStates.FindShelter;
		}

		if (!dialogueControl.Visible && GlobalPosition.DistanceTo(_targetPosition) > 5)
		{
			Movement(_targetPosition);
		}
		if(_taskStarted == true && _resourcheTaskCounter == 5)
		{
			ResourcheGatheringDone();
		}
	}

	public VillagerStates GetVillagerStates()
	{
		return _state;
	}

	void State()
	{
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

			case VillagerStates.FindResourchesTask:
				GatherResourches();
				break;

			case VillagerStates.GetHospitalized:
				break;

			case VillagerStates.FixFence:
				break;

			case VillagerStates.FindArcherTower:
				break;

			case VillagerStates.FindShelter:
				FindShelter();
				break;

			default: 
				GD.Print("State not found");
				break;
		}
		//GD.Print(_state);
	}

	public void _on_button_button_up()
	{
		//dialogueControl.Visible = true;
		dialogueWindow = true;
		_info.Visible = true;
		_info.UpdateStatus(_state);
	}

	public void _on_area_2d_area_entered(Area2D area)
	{
		collision = true;
		//GD.Print("Touching something");
	}

/* 	public void _on_gathering_timer_timeout()
	{
		ResourcheGatheringDone();
	} */

	void Movement(Vector2 target)
	{
		_speed = 100;
		Vector2 _direction = (target - GlobalPosition).Normalized();
		Velocity = _direction * _speed;
		MoveAndSlide();
	}

	void ChooseTask()
	{
		if (dialogueControl.farmingTaskStarted)
		{
			GD.Print("Farming task started");
			_state = VillagerStates.FarmingTask;
			dialogueWindow = false;
		}
 		if(dialogueControl.resourcheTaskStarted)
		{
			GD.Print("Finding resourches");
			_state = VillagerStates.FindResourchesTask;
			dialogueWindow = false;
		}
		if (dialogueControl.exitDialogue)
		{
			_state = VillagerStates.RoamAround;
			dialogueWindow = false;
			dialogueControl.exitDialogue = false;
		}
	}

	void FindShelter()
	{
		if (dialogueControl.Visible)
		{
			_state = VillagerStates.ChooseTask;
		}
		else{
			_state = VillagerStates.RoamAround;
		}
	}

	void RoamAround()
	{
		dialogueControl.resourcheTaskStarted = false;
		dialogueControl.farmingTaskStarted = false;
		float range = 100;
        _targetPosition = GlobalPosition + new Vector2(GD.Randf() * range * 8 - range, GD.Randf() * range * 8 - range);
		if (dialogueControl.Visible)
		{
			_state = VillagerStates.ChooseTask;
		}
	}

 	void GatherResourches()
	{
		_targetPosition = _streetSign.GlobalPosition;
		if (GlobalPosition.DistanceTo(_targetPosition) < 5)
		{
			_info.Visible = false;
			_taskStarted = true;
			_villagerSprite.Visible = false;
			_resourcheTaskCounter ++;
		}
	}

	void ResourcheGatheringDone()
	{
		Random rnd = new Random();
		_villagerSprite.Visible = true;
		if(dialogueControl.findStone == true)
		{
			int amount = rnd.Next(4, 21);
			GD.Print("Found: " + amount + " stone");
			dialogueControl.findStone = false;
		}
		if(dialogueControl.findWood == true)
		{
			int amount = rnd.Next(4, 21);
			GD.Print("Found: " + amount + " wood");
			dialogueControl.findWood = false;
		}
		_taskStarted = false;
		_resourcheTaskCounter = 0;
		dialogueControl.resourcheTaskStarted = false;
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
				dialogueControl.farmingTaskStarted = false;
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


