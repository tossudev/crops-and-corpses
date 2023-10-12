using Godot;
using System;
using System.Reflection;

public partial class ZombieChase : States
{
	[Export] private CharacterBody2D _zombie;
	[Export] private float _moveSpeed = 150.0f;
	private CharacterBody2D _player;

    public override void Enter()
    {
		_player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
    }

    public override void Physics_Update(double delta)
    {
		CheckIfPlayerAlive();

		if(_player != null)
		{
			Vector2 direction = _player.GlobalPosition - _zombie.GlobalPosition;

			if(direction.Length() > 85)
			{
				_zombie.Velocity = direction.Normalized() * _moveSpeed;
			}
			else
			{
				_zombie.Velocity = Vector2.Zero;
			}

			if (direction.Length() > 600)
			{
				EmitSignal("Transitioned", "idle");			
			}
		}
		else 
		{
			EmitSignal("Transitioned", "idle");
		}
    }

	private void CheckIfPlayerAlive()
	{
		if (RoamingZombie.playerAlive)
		{
			_player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
			if(_player == null)
			{
				RoamingZombie.playerAlive = false;
				EmitSignal("Transitioned", "idle");	
			}
		}
	}
}
