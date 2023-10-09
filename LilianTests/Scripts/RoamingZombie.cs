using Godot;
using System;

public partial class RoamingZombie : CharacterBody2D
{
	[Export] private float _damage;
	private Sprite2D _sprite;
	private CharacterBody2D _player;
	private Attack _attack;
	private HealthComponent _healthComponent;

    public override void _Ready()
    {
		_damage = 10.0f;
        _sprite = GetNode<Sprite2D>("Sprite2D");
		//_healthComponent = GetNode<HealthComponent>("HealthComponent");

		_attack = new Attack
		{
			damage = _damage
		};
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
		
		if (Velocity.X > 0)
    	{
    		_sprite.FlipH = false;
    	}
    	else
    	{
    		_sprite.FlipH = true;
    	}
    }

	private void OnAttackBoxEntered(Node2D body)
	{
		if(body.IsInGroup("player"))
		{
			_player = (CharacterBody2D)body;
			HitboxComponent _hitbox = _player.GetNodeOrNull<HitboxComponent>("HitboxComponent");

			if (_hitbox != null)
			{
                _hitbox.Damage(_attack);
			}
			else 
			{
				GD.Print("No hitbox found on player");
			}
		}
	}
}
