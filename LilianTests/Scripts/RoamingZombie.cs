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
	private NodePath _rootNodePath;
	private Node2D rootNode;
	PackedScene instantiatedNPC;

	public override void _Ready()
	{
		
		
		instantiatedNPC = (PackedScene)GD.Load("res://DaniTests/Scenes/dummyScene.tscn");
		_rootNodePath = GetParent<Node2D>().GetPath();
		rootNode = GetNodeOrNull<Node2D>(_rootNodePath);
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
		/* GD.Print("2");
		GD.Print(attack.effect); */
		switch (attack.effect)
				{
				case EffectType.Cure:

					Transform2D zombiePos = this.Transform;
					CharacterBody2D spawnNPC = (CharacterBody2D)instantiatedNPC.Instantiate();
					spawnNPC.Transform = zombiePos;
					rootNode.AddChild(spawnNPC);
					this.QueueFree();
					break;
				default:
					break;
				}
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