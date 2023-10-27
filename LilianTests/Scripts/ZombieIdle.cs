using Godot;
using System;

public partial class ZombieIdle : ZombieStates
{
	[Export] private CharacterBody2D _zombie;
	[Export] private float _moveSpeed = 100.0f;

	private Vector2 _moveDirection = Vector2.Zero;
	private double _roamTime;
	private CharacterBody2D _player;
	private Node2D _fences;
	
	public override void Enter()
    {
        _player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
		_fences = (Node2D)GetTree().GetFirstNodeInGroup("fences");
		RandomizeRoam();
    }

    public override void Update(double delta)
    {
        if(_roamTime > 0)
		{
			_roamTime -= delta;
		}
		else 
		{
			RandomizeRoam();
		}
    }

    public override void Physics_Update(double delta)
    {
	    if (_zombie == null) return;
	    
	    _zombie.Velocity = _moveDirection * _moveSpeed;

	    if (_player == null || !RoamingZombie.playerAlive) return;
	    
	    Vector2 playerDirection = _player.GlobalPosition - _zombie.GlobalPosition;
		Vector2 fenceDirection = _fences?.GlobalPosition - _zombie.GlobalPosition ?? Vector2.Zero;

		if(playerDirection.Length() < 300 )
		{
			EmitSignal("Transitioned", "chase");			
		}
		else if (fenceDirection.Length() < 1000)
		{
			EmitSignal("Transitioned", "attackfence");
		}
    }


	private void RandomizeRoam()
	{
		_moveDirection = new Vector2(
			(float)(GD.RandRange(0, 2001) - 1000) / 1000,
			(float)(GD.RandRange(0, 2001) - 1000) / 1000 // random range -1, 1
		);

		_roamTime = (float)(GD.RandRange(1000, 3001)) / 1000; // random range 1, 3

		//GD.Print("MOVE_DIR: " + _moveDirection + "\nROAM_TIME: " + _roamTime);
	}
}
