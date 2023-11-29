using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class ZombieAttackFence : ZombieStates
{
	// public bool inTown;
	[Export] private CharacterBody2D _zombie;
	[Export] private float _moveSpeed = 100.0f;
	[Export] private NavigationAgent2D _navAgent;
	private Vector2 _movementTargetPos = Vector2.Zero;
	private Vector2 _moveDirection = Vector2.Zero;
	private CharacterBody2D _player;
	private Node2D _fences;
	//AnimationPlayer animPlayer;

	public Vector2 MovementTarget
    {
        get { return _navAgent.TargetPosition; }
        set { _navAgent.TargetPosition = value; }
    }

	public override void Enter()
    {
        _player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
		_fences = (Node2D)GetTree().GetFirstNodeInGroup("fences");

		_movementTargetPos = _fences.GlobalPosition;

		_navAgent.PathDesiredDistance = 300.0f;
		_navAgent.TargetDesiredDistance = 4.0f;
		CallDeferred("ActorSetup");
    }

	public override void Physics_Update(double delta)
	{	
		if (_zombie == null) return;

		if (_navAgent.IsNavigationFinished())
        {
            EmitSignal("Transitioned", "idle");

			if(_zombie.HasMethod("SetInTown"))
			{
				_zombie.Call("SetInTown");
			}
        }

		Vector2 currentAgentPosition = _zombie.GlobalPosition;
        Vector2 nextPathPosition = _navAgent.GetNextPathPosition();

        Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        newVelocity *= _moveSpeed;
		//animPlayer.SpeedScale = _moveSpeed /100;
		_zombie.Velocity = newVelocity;

		Vector2 playerDirection = _player.GlobalPosition - _zombie.GlobalPosition;

		if(playerDirection.Length() < 300 )
		{
			EmitSignal("Transitioned", "chase");			
		}
		else if (ZombieManager.dayMode)
		{
			EmitSignal("Transitioned", "idle");
		}
	}

	private async void ActorSetup()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        MovementTarget = _movementTargetPos;
    }
}
