using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class ZombieAttackFence : ZombieStates
{
	// todo: add health and hitbox components to fences and somehow get zombies to know if theres a hole in the wall somewhere and head there
	// right now this should find the nearest fence to the zombie 
	// !!! not yet attached to the zombies itself, so doesnt affect the game yet.

	[Export] private CharacterBody2D _zombie;
	[Export] private float _moveSpeed = 100.0f;

	private Vector2 _moveDirection = Vector2.Zero;
	private double _roamTime;
	private CharacterBody2D _player;

	private static List<Node2D> _fenceList = new List<Node2D>();
	private Node2D _fences;
	private int _fenceCount;

	public override void Enter()
    {
        _player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
		_fences = (Node2D)GetTree().GetFirstNodeInGroup("fences");
		ListAllFences();
    }

	public override void Physics_Update(double delta)
	{	
		if (_zombie == null) return;

		_zombie.Velocity = _moveDirection * _moveSpeed;

		Vector2 fenceDirection = NearestFence();
		Vector2 playerDirection = _player.GlobalPosition - _zombie.GlobalPosition;

		if(playerDirection.Length() < 300 )
		{
			EmitSignal("Transitioned", "chase");			
		}
		else if(fenceDirection.Length() > 300)
		{
			EmitSignal("Transitioned", "idle");
		}		
	}

	private void ListAllFences()
	{
		if(_fences != null)
		{
			foreach (Node2D fence in _fences.GetChild(0).GetChildren())
			{
				_fenceList.Add(fence);
			}
			_fenceCount = _fenceList.Count;
		}
	}

	private Vector2 NearestFence()
	{
		float shortestDistance = 5000f; // some big value at first
		Node2D closestFence = null;

		foreach (Node2D fence in _fenceList)
		{
			Vector2 fenceToZombie = _zombie.GlobalPosition - fence.GlobalPosition;
			float distance = fenceToZombie.Length();

			if(distance < shortestDistance)
			{
				shortestDistance = distance;
				closestFence = fence;
			}
		}
		
		//GD.Print("closest : " + closestFence.Name);
		return closestFence.GlobalPosition - _zombie.GlobalPosition;
	}
}
