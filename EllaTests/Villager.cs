using Godot;
using System;
using System.Collections;
using System.Diagnostics;
using static VillagerManager;

public partial class Villager : CharacterBody2D
{
	VillagerStates _state;
	Vector2 _targetPosition;
	Timer _timer;
	Timer _resourcheGatherTime;

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
	public bool dialogueWindow = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_streetSign = GetParent().GetParent().GetNode<Node2D>("StreetSignSpot");
		_villagerSprite = GetNode<Sprite2D>("Sprite2D");
		//_resourcheGatherTime = GetNode<Timer>("Timer");

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
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!dialogueControl.Visible && GlobalPosition.DistanceTo(_targetPosition) > 5)
		{
			Movement(_targetPosition);
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
				break;
		}
	}

	public void _on_button_button_up()
	{
		dialogueWindow = true;
	}

	public void _on_area_2d_area_entered(Area2D area)
	{
		collision = true;
		//GD.Print("Touching something");
	}

	public void _on_timer_timeout()
	{
		_timerStopped = true;
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
		dialogueControl.resourcheTaskStarted = false;
		dialogueControl.farmingTaskStarted = false;
		//navMeshAgent.TargetPosition = _targetPosition;

		// Don't delete
		float range = 100;
		_targetPosition = GlobalPosition + new Vector2(GD.Randf() * range * 8 - range, GD.Randf() * range * 8 - range);

		//_targetPosition = RandomTargetPosition();
		Vector2 direction = _targetPosition - GlobalPosition;
		Vector2 oppDirection = -direction;
		if(collision)
		{
			_targetPosition = oppDirection;
			collision = false;
		}

		if (dialogueControl.Visible)
		{
			_state = VillagerStates.ChooseTask;
			State();
		}
	}


	//Incase u reading this and wondering what is this Im testing diff movement bc its **** 
	Vector2 RandomTargetPosition()
	{
		float rangeX = 1500;
		float rangeY = 2000;
		float randomX = (GD.Randf() * rangeX);
		float randomY = (GD.Randf() * rangeY);

		return new Vector2(randomX, randomY);
	}

	void ChooseTask()
	{
		GD.Print("Choosing task");
		if (dialogueControl.farmingTaskStarted)
		{
			GD.Print("Farming task started");
			_state = VillagerStates.FarmingTask;
			State();
		}
 		if(dialogueControl.resourcheTaskStarted)
		{
			GD.Print("Finding resourches");
			_state = VillagerStates.FindResourchesTask;
			State();
		}
		if (dialogueControl.exitDialogue)
		{
			_state = VillagerStates.RoamAround;
			State();
			dialogueControl.exitDialogue = false;
		}
	}

 	void GatherResourches()
	{
		bool _taskStarted = false;
		_targetPosition = _streetSign.GlobalPosition;
		if (GlobalPosition.DistanceTo(_targetPosition) < 5)
		{
			_taskStarted = true;
			_villagerSprite.Visible = false;
		}
		if(_taskStarted)
		{
			_resourcheGatherTime = new Timer{
				WaitTime = 5f,
			};
			AddChild(_resourcheGatherTime);
			_resourcheGatherTime.Start();
			_resourcheGatherTime.Timeout += _on_timer_timeout;

			if(_timerStopped == true)
			{
				GD.Print("timerstopped");
				_resourcheGatherTime.QueueFree();
				_resourcheGatherTime.Free();
				_timerStopped = false;
				_villagerSprite.Visible = true;
				_state = VillagerStates.RoamAround;
			} 
		}
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
		//not working (yet)
		_targetPosition = GetParent().GetNode<CharacterBody2D>("Forest/Objects/Player").GlobalPosition;
	}
}
