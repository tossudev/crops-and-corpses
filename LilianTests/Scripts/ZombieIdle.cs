using Godot;
using System;

public partial class ZombieIdle : States
{
	[Export] private CharacterBody2D _zombie;
	[Export] private float _moveSpeed = 100.0f;

	private Vector2 _moveDirection = Vector2.Zero;
	private double _roamTime;
	private CharacterBody2D _player;
	
	public override void Enter()
    {
        _player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
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
        if(_zombie != null)
		{
			_zombie.Velocity = _moveDirection * _moveSpeed;
		}
		
		if(_player != null)
		{
			Vector2 direction = _player.GlobalPosition - _zombie.GlobalPosition;

			if(direction.Length() < 300 )
			{
				EmitSignal("Transitioned", "chase");			
			}
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
