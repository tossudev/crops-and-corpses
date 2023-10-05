using Godot;
using System;
using System.Reflection;

public partial class ZombieChase : States
{
	[Export] public CharacterBody2D zombie;
	[Export] public float move_speed = 50.0f;

	// private Attack attack;
	private CharacterBody2D player;

    public override void Enter()
    {
		player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
    }

    public override void Physics_Update(double delta)
    {
		if(player != null)
		{
			Vector2 direction = player.GlobalPosition - zombie.GlobalPosition;

			if(direction.Length() > 85)
			{
				zombie.Velocity = direction.Normalized() * move_speed;
			}
			else
			{
				//GD.Print("rawwr");
				zombie.Velocity = Vector2.Zero;
			}

			if (direction.Length() > 600)
			{
				EmitSignal("Transitioned", "idle");			
			}
		}
    }
}
