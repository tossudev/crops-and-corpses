using Godot;
using System;
using System.Diagnostics.Tracing;

public partial class RoamingZombie : CharacterBody2D
{
	[Export] private float _damage;
	private Sprite2D _sprite;
	private CharacterBody2D _player;
	private HitboxComponent _hitbox;
	private Attack _attack;
	private Timer _timer;
	private bool _playerInAttackRange;
	private HealthComponent _healthComponent;
	[Export] private NodePath _characterNodePath = null;
	private CharacterBody2D _thisZombie;
	PackedScene packedScene;

	public override void _Ready()
	{
		packedScene = (PackedScene)GD.Load("res://LilianTests/Prefabs/zombie_with_hitbox.tscn");

		_thisZombie = GetNodeOrNull<CharacterBody2D>(_characterNodePath);
		_damage = 10.0f;
		_playerInAttackRange = false;
		_sprite = GetNode<Sprite2D>("Sprite2D");
		//_healthComponent = GetNode<HealthComponent>("HealthComponent");

		_attack = new Attack
		{
			damage = _damage,
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();

		// if (_playerInAttackRange)
		// {
		// 	//AttackPlayer(_hitbox);
		// 	_timer.Start();
		// }
		// else
		// {
		// 	_timer.Stop();
		// }

		if (Velocity.X > 0)
		{
			_sprite.FlipH = false;
		}
		else
		{
			_sprite.FlipH = true;
		}
	}

	private void AttackReceived(Attack attack)
	{
		GD.Print("2");
		GD.Print(attack.damage);
	}

	private void OnAttackBoxEntered(Node2D body)
	{
		GD.Print("!!!!!!!!!");
		if (body.IsInGroup("player"))
		{
			_player = (CharacterBody2D)body;
			_hitbox = _player.GetNodeOrNull<HitboxComponent>("HitboxComponent");

			if (_hitbox != null)
			{
				_hitbox.ApplyAttack(_attack);
				// _playerInAttackRange = true;
				// //AttackPlayer(_hitbox);

				switch (_attack.effect)
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

	// private void OnAttackBoxExited(Node2D body)
	// {
	// 	_playerInAttackRange = false;
	// }

	// private void OnTimerTimeout()
	// {
	// 	GD.Print("attack player");
	// 	if(_hitbox != null)
	// 	{
	// 		_hitbox.ApplyAttack(_attack);
	// 	}

	// }
}