using Godot;
using System;
using System.Reflection;

public partial class ZombieChase : States
{
	[Export]
	public CharacterBody2D zombie;
	[Export]
	public float move_speed = 50.0f;

	private CharacterBody2D player;

    public override void Enter()
    {
        player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
    }

    public override void Physics_Update(double delta)
    {
        Vector2 direction = player.GlobalPosition - zombie.GlobalPosition;

		if(direction.Length() > 25)
		{
			zombie.Velocity = direction.Normalized() * move_speed;
		}
		else
		{
			zombie.Velocity = Vector2.Zero;
		}

		if (direction.Length() > 50)
		{
			GD.Print("IDLATAAN");
			EmitSignal("Transitioned", "idle");			
		}
    }
}
