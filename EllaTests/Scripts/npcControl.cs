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
		Dialogue


	}
	public States CurrentState;
	public NavigationAgent2D navigationAgent;
	public float speed = 90;
	List<Marker2D> _waypointsFarm = new List<Marker2D>();
	List<Marker2D> _waypointsTask = new List<Marker2D>();
	List<Marker2D> _waypointTaskCompleted = new List<Marker2D>();
	int _waypointIndex;
	Vector2 _targetPos;
	Vector2 _direction;
	Timer _taskTimer;
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

		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		// Access to group
		_waypointsFarm = GetTree().GetNodesInGroup("WaypointFarm").Select(saar => saar as Marker2D).ToList();
		_waypointsTask = GetTree().GetNodesInGroup("Taskpoint").Select(saar => saar as Marker2D).ToList();
		_waypointTaskCompleted = GetTree().GetNodesInGroup("TaskCompleted").Select(saar => saar as Marker2D).ToList();

		CurrentState = States.Patrol;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		switch (CurrentState)
		{
			case States.Patrol:
				speed = 90;

				_direction = GlobalPosition.DirectionTo(_targetPos);
				_targetPos = navigationAgent.TargetPosition;
				Velocity = _direction * speed;
				MoveAndSlide();

				if (Input.IsActionJustPressed("Click"))
				{
					GD.Print("Task1 started");
					CurrentState = States.Task;
					_taskTimer.Start();
				}

				if (navigationAgent.IsNavigationFinished())
				{
					MoveToWaypoints();
					return;
				}

				break;

			case States.Task:

				_direction = GlobalPosition.DirectionTo(_targetPos);
				_targetPos = navigationAgent.TargetPosition;
				Velocity = _direction * speed;
				MoveAndSlide();

				if (navigationAgent.IsNavigationFinished() && !_taskTimer.IsStopped())
				{
					MoveToWaypoints();
					return;
				}

				if (_taskTimer.IsStopped())
				{
					TaskCompleted();
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
		}
	}

	private void MoveToWaypoints()
	{
		if (CurrentState == States.Patrol)
		{
			GD.Print(_waypointIndex);
			_waypointIndex += 1;
			if (_waypointIndex > _waypointsFarm.Count - 1)
			{
				_waypointIndex = 0;
			}
			navigationAgent.TargetPosition = _waypointsFarm[_waypointIndex].GlobalPosition;
		}

		if (CurrentState == States.Task)
		{
			GD.Print(_waypointIndex);
			_waypointIndex += 1;
			if (_waypointIndex > _waypointsTask.Count - 1)
			{
				_waypointIndex = 0;
			}
			navigationAgent.TargetPosition = _waypointsTask[_waypointIndex].GlobalPosition;
		}
	}

	private void TaskCompleted()
	{
		navigationAgent.TargetPosition = _waypointTaskCompleted[0].GlobalPosition;
		if (GlobalPosition.DistanceTo(_waypointTaskCompleted[0].GlobalPosition) < 1.0)
		{
			speed = 0;
		}
	}
}
