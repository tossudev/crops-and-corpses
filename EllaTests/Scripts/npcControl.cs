using Godot;
using System;
using System.Collections;
using System.Diagnostics;

public partial class npcControl : CharacterBody2D
{
	public enum States
	{
		Patrol,
		TaskFarming,
		TaskDefend,
		TaskGather,
		TaskCompleted,
		FollowPlayer

	}
	public States CurrentState;
	public bool dialogueWindow = false;
	bool _taskCompleted;
	float _speed = 100;
	Timer _taskTimer;
	Vector2 _targetPosition;
	[Export]
	DialogueControl dialogueControl;

	Plant _currentPlant;
	int _plantIndex = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("im ready");


		// Setting up the timer
		_taskTimer = new Timer
		{
			WaitTime = 5f,

		};
		_taskTimer.Timeout += NpcStates;

		AddChild(_taskTimer);
		_taskTimer.Start();

		CurrentState = States.Patrol;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.DistanceTo(_targetPosition) > 5 && !dialogueControl.Visible)
		{
			Movement(_targetPosition);
		}
		if (CurrentState == States.TaskFarming && GlobalPosition.DistanceTo(_targetPosition) < 5)
		{
			NpcStates();
		}
	}

	void NpcStates()
	{
		GD.Print(CurrentState);
		switch (CurrentState)
		{
			case States.Patrol:
				TargetPosition();
				if (dialogueControl.farmingTaskStarted == true)
				{
					GD.Print("Farming task started");
					CurrentState = States.TaskFarming;
					NpcStates();
					break;
				}

				if (GlobalPosition.DistanceTo(_targetPosition) < 10)
				{
					TargetPosition();
					break;
				}

				break;

			case States.TaskFarming:

				TargetPosition();
				dialogueControl.farmingTaskStarted = false;
				CheckPlant();

				if (_taskCompleted == true)
				{
					CurrentState = States.TaskCompleted;
				}
				if (dialogueControl.exitDialogue == true)
				{
					dialogueControl.exitDialogue = false;
				}

				break;

			case States.TaskDefend:
				TargetPosition();
				dialogueControl.attackZombies = false;

				break;

			case States.TaskCompleted:
				// Add here farming resourches
				_taskCompleted = false;
				TargetPosition();

				if (dialogueControl.farmingTaskStarted == true)
				{
					GD.Print("Farming task started");
					CurrentState = States.TaskFarming;
					_taskTimer.Start();
					dialogueControl.farmingTaskStarted = false;
				}
				if (dialogueControl.exitDialogue == true)
				{
					CurrentState = States.Patrol;
					dialogueControl.exitDialogue = false;
				}
				break;

			case States.FollowPlayer:
				TargetPosition();

				break;
		}
	}
	private void TargetPosition()
	{
		switch (CurrentState)
		{
			case States.Patrol:
				float range = 100;
				_targetPosition = GlobalPosition + new Vector2(GD.Randf() * range * 8 - range, GD.Randf() * range * 8 - range);
				break;
			case States.TaskFarming:
				_currentPlant = FarmManager.instance.GetPlantedPlants()[_plantIndex];
				_targetPosition = _currentPlant.GlobalPosition;
				break;
			case States.TaskDefend:
				_targetPosition = GetParent().GetNode<CharacterBody2D>("zombie").GlobalPosition;
				break;
			case States.TaskGather:
				break;
			case States.FollowPlayer:
				_targetPosition = GetParent().GetNode<CharacterBody2D>("Player").GlobalPosition;
				break;
		}
	}

	private void Movement(Vector2 target)
	{
		_speed = 100;
		Vector2 _direction = (target - GlobalPosition).Normalized();
		Velocity = _direction * _speed;
		MoveAndSlide();
	}
	public void _on_button_button_up()
	{
		dialogueWindow = true;

		if (_targetPosition == GetParent().GetNode<Marker2D>("TaskCompleted").GlobalPosition)
		{
			dialogueControl.farmingTaskStarted = false;
			GD.Print("Task1 Completed");
			CurrentState = States.TaskCompleted;
		}
	}

	private void CheckPlant()
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
				CurrentState = States.Patrol;
			}
		}
	}
}