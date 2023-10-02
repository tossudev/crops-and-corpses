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
	private List<Marker2D> waypointsFarm = new List<Marker2D>();
	private int waypointIndex;
	Vector2 targetPos;
	Vector2 direction;
	public float speed = 50;

	public float timerStart;
	public float timerInterval = 40;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timerStart = 0;
		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		waypointsFarm = GetTree().GetNodesInGroup("WaypointFarm").Select(saar => saar as Marker2D).ToList();

		CurrentState = States.Patrol;
		navigationAgent.TargetPosition = waypointsFarm[0].GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		switch (CurrentState)
		{
			case States.Patrol:

				if (navigationAgent.IsNavigationFinished())
				{
					MoveToWaypoints();
					return;
				}

				if (Input.IsKeyPressed(Key.E))
				{
					CurrentState = States.Task;
				}

				GD.Print(waypointIndex);

				direction = GlobalPosition.DirectionTo(targetPos);
				targetPos = navigationAgent.TargetPosition;
				Velocity = direction * speed;
				MoveAndSlide();

				break;

			case States.Task:
				GD.Print("Task1 started");
				break;

			case States.Dialogue:
				break;
		}
	}

	private void MoveToWaypoints()
	{
		waypointIndex += 1;
		if (waypointIndex > waypointsFarm.Count - 1)
		{
			waypointIndex = 0;
		}
		navigationAgent.TargetPosition = waypointsFarm[waypointIndex].GlobalPosition;
	}


}
