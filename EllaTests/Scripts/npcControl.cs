using Godot;
using System;

public partial class npcControl : CharacterBody2D
{
	public enum States
	{
		Patrol,
		Task,
		TaskCompleted,
		FollowPlayer

	}
	public States CurrentState;
	public bool dialogueWindow = false;
	bool _taskCompleted;
	int _waypointIndex;
	float _speed = 50;
	Timer _taskTimer;
	Vector2 _targetPosition;
	[Export]
	DialogueControl dialogueControl;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Setting up the timer
		_taskTimer = new Timer
		{
			WaitTime = 30f,
			OneShot = true
		};

		AddChild(_taskTimer);

		// If npc in outside world 
		// CurrentState = States.FollowPlayer;

		//Else:
		CurrentState = States.Patrol;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		switch (CurrentState)
		{
			case States.Patrol:

				if (Position.DistanceTo(_targetPosition) < 10)
				{
					TargetPosition();
				}
				Movement(_targetPosition);

				if (dialogueControl.farmingTaskStarted == true)
				{
					GD.Print("Farming task started");
					CurrentState = States.Task;
					_taskTimer.Start();
				}

				break;

			case States.Task:

				TargetPosition();
				Movement(_targetPosition);
				dialogueControl.farmingTaskStarted = false;
				if(_taskTimer.IsStopped()){
					_taskCompleted = true;
	
				}
				if(_taskCompleted == true)
				{
					CurrentState = States.TaskCompleted;
				}
				if(dialogueControl.exitDialogue == true)
				{
					dialogueControl.exitDialogue = false;
				}

				break;

			case States.TaskCompleted:
				//_speed = 0;
				_taskCompleted = false;
				//Add resources to player inventory or something
				TargetPosition();
				Movement(_targetPosition);
				if (dialogueControl.farmingTaskStarted == true)
				{
					GD.Print("Farming task started");
					CurrentState = States.Task;
					_taskTimer.Start();
					dialogueControl.farmingTaskStarted = false;
				} 
				if(dialogueControl.exitDialogue == true)
				{
					CurrentState = States.Patrol;
					dialogueControl.exitDialogue = false;
				}
				break;

			case States.FollowPlayer:
				TargetPosition();
				Movement(_targetPosition);

				break;
		}
	}

	private void TargetPosition()
	{
		if (CurrentState == States.Patrol)
		{
			float range = 100;
			_targetPosition = GlobalPosition + new Vector2(GD.Randf() * range * 8 - range, GD.Randf() * range * 8 - range);
		}
		if (CurrentState == States.Task)
		{
			_targetPosition = GetParent().GetNode<Marker2D>("TaskPoint").GlobalPosition;

		}
		if(CurrentState == States.TaskCompleted)
		{
			_targetPosition = GetParent().GetNode<Marker2D>("TaskCompleted").GlobalPosition;
		}
		if (CurrentState == States.FollowPlayer)
		{
			//_targetPosition = GetParent().GetNode<CharacterBody2D>("Player").GlobalPosition;
		}
	}

	private void Movement(Vector2 target)
	{
		_speed = 50;
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
}