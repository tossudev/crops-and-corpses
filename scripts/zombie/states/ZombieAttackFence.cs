using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class ZombieAttackFence : ZombieStates
{
	// todo: add health and hitbox components to fences
	public bool inTown;
	[Export] private CharacterBody2D _zombie;
	[Export] private float _moveSpeed = 100.0f;
	[Export] private NavigationAgent2D _navAgent;
	private Vector2 _movementTargetPos = Vector2.Zero;
	private Vector2 _moveDirection = Vector2.Zero;
	private double _roamTime;
	private CharacterBody2D _player;
	private static List<Node2D> _fenceList = new List<Node2D>();
	private Node2D _fences;
	private int _fenceCount;
	[Export] AnimationPlayer animPlayer;

	public Vector2 MovementTarget
    {
        get { return _navAgent.TargetPosition; }
        set { _navAgent.TargetPosition = value; }
    }

	public override void Enter()
    {
        _player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
		_fences = (Node2D)GetTree().GetFirstNodeInGroup("fences");
 		_movementTargetPos = ZombieManager.moveTarget.Position;
		ListAllFences();

		_navAgent.PathDesiredDistance = 4.0f;
		_navAgent.TargetDesiredDistance = 4.0f;
		CallDeferred("ActorSetup");
    }

	public override void Physics_Update(double delta)
	{	
		// if (_zombie == null) return;
		if (_navAgent.IsNavigationFinished())
        {
			// for now
			// what the hell will the zombie do once inside the town walls????
			// inTown = true; ???
            EmitSignal("Transitioned", "idle");
        }

		Vector2 currentAgentPosition = _zombie.GlobalPosition;
        Vector2 nextPathPosition = _navAgent.GetNextPathPosition();

        Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        newVelocity *= _moveSpeed;
		_zombie.Velocity = newVelocity;

		//Vector2 fenceDirection = NearestFence();
		Vector2 playerDirection = _player.GlobalPosition - _zombie.GlobalPosition;

		if(playerDirection.Length() < 300 )
		{
			EmitSignal("Transitioned", "chase");			
		}
		// else if(fenceDirection.Length() > 300)
		// {
		// 	EmitSignal("Transitioned", "idle");
		// }		
	}

	private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        MovementTarget = _movementTargetPos;

		// On the first frame the NavigationServer map has not synchronized region data and any path query will return empty.
		// Await one frame to pause scripts until the NavigationServer had time to sync.
    }

	private void ListAllFences()
	{
		// Node2D fences = (Node2D)GetTree().GetRoot().GetNode("town/fences");
		if(_fences != null)
		{
			foreach (Node2D fence in _fences.GetChild(0).GetChildren())
			{
				_fenceList.Add(fence);
			}
			_fenceCount = _fenceList.Count;
		}
	}

}
