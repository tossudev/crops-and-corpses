using Godot;
using System;

public partial class ZombieChase : ZombieStates
{
	[Export] private CharacterBody2D _zombie;
	[Export] private float _moveSpeed = 150.0f;
	private CharacterBody2D _player;
	private Node2D _fences;

    public override void Enter()
    {
		_player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
		_fences = (Node2D)GetTree().GetFirstNodeInGroup("fences");

		if (_zombie != null && _zombie.HasMethod("ChangeZombieNoise"))
		{
			_zombie.CallDeferred("ChangeZombieNoise", ZombieManager._hiss1);
		}		
    }

    public override void Physics_Update(double delta)
    {
		CheckIfPlayerAlive();

		if(_player != null)
		{
			_moveSpeed = ZombieManager.chaseSpeed;
			Vector2 playerDirection = _player.GlobalPosition - _zombie.GlobalPosition;
			Vector2 fenceDirection = _fences?.GlobalPosition - _zombie.GlobalPosition ?? Vector2.Zero;
			
			if(playerDirection.Length() > 85)
			{
				_zombie.Velocity = playerDirection.Normalized() * _moveSpeed;
			}
			else 
			{
				_zombie.Velocity = Vector2.Zero;
			}

			if (playerDirection.Length() > 600)
			{
				// if (fenceDirection.Length() < 1000)
				// {
				// 	EmitSignal("Transitioned", "attackfence");
				// }
				// else 
				// {
					EmitSignal("Transitioned", "idle");	
				// }						
			}
		} 
		else 
		{
			EmitSignal("Transitioned", "idle");
		}
    }

	private void CheckIfPlayerAlive()
	{
		if (!ZombieManager.playerAlive)
		{
			// _player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
			// if(_player == null)
			// {
			// 	ZombieManager.playerAlive = false;
				EmitSignal("Transitioned", "idle");	
		// 	}
		}
	}
}
