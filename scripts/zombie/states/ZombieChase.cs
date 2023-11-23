using Godot;
using System;

public partial class ZombieChase : ZombieStates
{
	[Export] private CharacterBody2D _zombie;
	[Export] private float _moveSpeed = 150.0f;
	[Export] private NavigationAgent2D _navAgent;
	private Vector2 _movementTargetPos = Vector2.Zero;
	private Timer _timer;
	private CharacterBody2D _player;
	private Node2D _fences;
	[Export]AnimationPlayer animPlayer;

	public Vector2 MovementTarget
    {
        get { return _navAgent.TargetPosition; }
        set { _navAgent.TargetPosition = value; }
    }
    public override void Enter()
    {
		_player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
		_fences = (Node2D)GetTree().GetFirstNodeInGroup("fences");		
		_timer = GetNodeOrNull<Timer>("Timer");

		if (_zombie != null)
		{
			ZombieManager.PlayZombieNoise(ZombieNoises.ZOMBIE_HISS_1);
		}	

		_navAgent.PathDesiredDistance = 80.0f;
		_navAgent.TargetDesiredDistance = 4.0f;
		_timer.Start();
    }

    public override void Physics_Update(double delta)
    {
		if(_player != null)
		{
			_moveSpeed = ZombieManager.chaseSpeed;
			Vector2 playerDirection = _player.GlobalPosition - _zombie.GlobalPosition;
			Vector2 fenceDirection = _fences?.GlobalPosition - _zombie.GlobalPosition ?? Vector2.Zero;
			
			if(playerDirection.Length() > 80)
			{
				ChasePlayer();
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
					_timer.Stop();
					EmitSignal("Transitioned", "idle");	
				// }						
			}
		} 
		else 
		{
			_timer.Stop();
			EmitSignal("Transitioned", "idle");
		}
    }

	private void ChasePlayer()
	{
		Vector2 currentAgentPosition = _zombie.GlobalPosition;
		Vector2 nextPathPosition = _navAgent.GetNextPathPosition();

		Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
		_zombie.Velocity = newVelocity * _moveSpeed;
		
		animPlayer.SpeedScale = 1*( _moveSpeed / 100 );
	}

	private void OnTimerTimeout()
	{
		CheckIfPlayerAlive();
		MovementTarget = _player.GlobalPosition;
	}

	private void CheckIfPlayerAlive()
	{
		if (!ZombieManager.playerAlive)
		{
			EmitSignal("Transitioned", "idle");	
		}
	}
}
