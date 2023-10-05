using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public partial class npcControl : CharacterBody2D
{
	public enum States
	{
		Patrol,
		Task,
		Dialogue,
		FollowPlayer

	}
	public States CurrentState;
	int _waypointIndex;
	Timer _taskTimer;
	Vector2 _targetPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Setting up the timer
		_taskTimer = new Timer
		{
			WaitTime = 20f,
			OneShot = true
		};

		AddChild(_taskTimer);

		/*Access to group DO NOT DELETE 
		_waypointsFarm = GetTree().GetNodesInGroup("WaypointFarm").Select(saar => saar as Marker2D).ToList();
		//List<Marker2D> _waypointTaskCompleted = new List<Marker2D>();
		*/

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

				if (Input.IsActionJustPressed("Click"))
				{
					GD.Print("Task1 started");
					CurrentState = States.Task;
					_taskTimer.Start();
				}

				break;

			case States.Task:

				TargetPosition();
				Movement(_targetPosition);


				if (_targetPosition == GetParent().GetNode<Marker2D>("TaskCompleted").GlobalPosition)
				{
					if (Input.IsActionJustPressed("Click"))
					{
						GD.Print("Task1 Completed");
						CurrentState = States.Patrol;
					}
				}

				break;

			case States.Dialogue:
				// Adding dialogue window where player can choose which task is being done
				// After that goes to task state
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
			_targetPosition = GlobalPosition + new Vector2
			(GD.Randf() * range * 2 - range, GD.Randf() * range * 2 - range);
		}
		if (CurrentState == States.Task)
		{
			//_targetPosition = _waypointsTask[0].GlobalPosition;
			_targetPosition = GetParent().GetNode<Marker2D>("TaskPoint").GlobalPosition;

			if (_taskTimer.IsStopped())
			{
				_targetPosition = GetParent().GetNode<Marker2D>("TaskCompleted").GlobalPosition;
			}
		}
		if (CurrentState == States.FollowPlayer)
		{
			//_targetPosition = GetParent().GetNode<CharacterBody2D>("Player").GlobalPosition;
		}
	}

	private void Movement(Vector2 target)
	{
		float _speed = 50;
		Vector2 _direction = (target - GlobalPosition).Normalized();
		Velocity = _direction * _speed;
		MoveAndSlide();
	}
}