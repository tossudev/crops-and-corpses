using Godot;
using System;

public partial class RoamingZombie : CharacterBody2D
{
	[Export] private float _damage;
	private Sprite2D sprite;
	private CharacterBody2D player;
	private Attack _attack;

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

	private void OnAttackBoxEntered(Node2D body)
	{
		if(body.IsInGroup("player"))
		{
			player = (CharacterBody2D)body;
			HitboxComponent hitbox = player.GetNode<HitboxComponent>("HitboxComponent");

			//GD.Print("hB: " + hitbox.Name);
			//hitbox.Damage(_attack);
		}
	}
}
