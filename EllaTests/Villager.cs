using Godot;
using System;
using static VillagerManager;

public partial class Villager : CharacterBody2D
{
	VillagerStates _state;
	TaskType _task;
	Vector2 _targetPosition;
	Timer _timer;
	DialogueControl dialogueControl;
	Plant _currentPlant;
	int _plantIndex = 0;
	float _speed = 0;
	public bool dialogueWindow = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = new Timer
		{
			WaitTime = 5f,
		};
		_timer.Timeout += State;
		AddChild(_timer);
		_timer.Start();

		_state = VillagerStates.RoamAround;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Movement(_targetPosition);
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
				break;

			case VillagerStates.ChooseTask:
				ChooseTask();
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

	void Task()
	{
		switch (_task)
		{
			case TaskType.FarmingTask:
				CheckPlants();
				break;

			case TaskType.FindResourchesTask:
				break;
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
		Movement(_targetPosition);
		ChooseTask();

	}

	void ChooseTask()
	{
		if (dialogueControl.farmingTaskStarted == true)
		{
			GD.Print("Farming task started");
			_task = TaskType.FarmingTask;
			Task();
		}
	}

	void CheckPlants()
	{
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
			/* 			if(_currentPlant.GetGrowthState() == GrowthState.IsDead)
						{
							_currentPlant.myField.RemovePlant();
						} */

			if (_currentPlant.GetGrowthState() != GrowthState.IsWilting && _plantIndex < FarmManager.instance.GetPlantedPlants().Count ||
			 _currentPlant.GetGrowthState() != GrowthState.WaitWatering && _plantIndex < FarmManager.instance.GetPlantedPlants().Count ||
			_currentPlant.GetGrowthState() != GrowthState.IsInfested && _plantIndex < FarmManager.instance.GetPlantedPlants().Count)
			{
				_plantIndex++;
				//_currentPlant = FarmManager.instance.GetPlantedPlants()[_plantIndex];

			}
			if (_plantIndex == FarmManager.instance.GetPlantedPlants().Count)
			{
				_plantIndex = 0;
			}
			if (_currentPlant.GetGrowthState() == GrowthState.IsHarvestable)
			{
				_plantIndex++;
				if (_plantIndex == FarmManager.instance.GetPlantedPlants().Count)
				{
					_state = VillagerStates.RoamAround;
				}
			}
		}
	}
}
