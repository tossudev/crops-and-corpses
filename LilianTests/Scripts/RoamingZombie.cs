using Godot;
using System;

public partial class RoamingZombie : CharacterBody2D
{
	[Export] private float _damage;
	private Sprite2D _sprite;
	private CharacterBody2D _player;
	private Attack _attack;
	private HealthComponent _healthComponent;
	[Export] private NodePath _characterNodePath = null;
	private CharacterBody2D _thisZombie;
	PackedScene packedScene;

    public override void _Ready()
    {
		packedScene = (PackedScene)GD.Load("res://LilianTests/Prefabs/zombie_with_hitbox.tscn");

		_thisZombie = GetNodeOrNull<CharacterBody2D>(_characterNodePath);
		//attack.damage = 10.0f;
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
				_hitbox.ApplyAttack(_attack);

				switch(_attack.effect)
				{
					case EffectType.Cure:
					
						Transform2D zombiePos = _thisZombie.Transform;
						_thisZombie.QueueFree();
						CharacterBody2D spawnNPC = (CharacterBody2D)packedScene.Instantiate();
						spawnNPC.Transform = zombiePos;
						AddChild(spawnNPC);
						break;
					default:
					break;

				}
               	 
			}
			else 
			{
				GD.Print("No hitbox found on player");
			}
		}
	}
}
