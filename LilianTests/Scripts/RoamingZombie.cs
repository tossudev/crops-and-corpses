using Godot;
using System;

public partial class RoamingZombie : CharacterBody2D
{
	private Sprite2D sprite;

	private Attack attack;

    public override void _Ready()
    {
		//attack.damage = 10.0f;
        sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
		
		if (Velocity.X > 0)
    	{
    		sprite.FlipH = false;
    	}
    	else
    	{
    		sprite.FlipH = true;
    	}
    }

	private void OnHitboxEntered(Area2D body)
	{
		GD.Print("HIT");
		if (body is HitboxComponent hitbox)
        {
            hitbox.Damage(attack);
        }		
	}
}
