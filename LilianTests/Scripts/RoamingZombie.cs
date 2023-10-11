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
	private ProgressBar _healthBar;
	private HealthComponent _healthComponent;
	private NodePath _rootNodePath;
	private Node2D rootNode;

	public static bool playerAlive = true; // this is not the best way for handling the player death but it'll do the job for now i guess
	PackedScene instantiatedNPC;

	public override void _Ready()
	{		
		instantiatedNPC = (PackedScene)GD.Load("res://EllaTests/npc.tscn");
		// DANIEL: changed _rootNodePath = GetParent<Node2D>().GetPath(); into the following since this returned a 'Godot.Window' idk it gave me an error
		_rootNodePath = GetPath();
		rootNode = GetNodeOrNull<Node2D>(_rootNodePath);		
		_damage = 5.0f;
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		_timer = GetNodeOrNull<Timer>("AttackTimer");
		_healthBar = GetNodeOrNull<ProgressBar>("HealthBar");
		_healthComponent = GetNodeOrNull<HealthComponent>("HealthComponent");

		_attack = new Attack
		{
			damage = _damage,
			knockback = 100f
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
		UpdateHealth();

		if (_sprite != null)
		{
			if (Velocity.X > 0)
			{
				_sprite.FlipH = false;
			}
			else
			{
				_sprite.FlipH = true;
			}
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
		if (body.IsInGroup("player"))
		{
			_player = (CharacterBody2D)body;
			_hitbox = _player.GetNodeOrNull<HitboxComponent>("HitboxComponent");

			if (_hitbox != null)
			{
				//_hitbox.ApplyAttack(_attack);
				_timer.Start();
			}
			else
			{
				GD.Print("No hitbox found on player");
			}
		}
	}

	private void UpdateHealth()
	{
		// update health bar when player damages zombie
		if (_healthComponent != null)
		{
			_healthBar.Value = _healthComponent._health;

			if (_healthComponent._health >= 100)
			{
				_healthBar.Visible = false;
			}
			else 
			{
				_healthBar.Visible = true;
			}
		}
	}

	private void OnAttackBoxExited(Node2D body)
	{
		_timer.Stop();
	}

	private void OnTimerTimeout()
	{
		//GD.Print("attack player");
		if(_hitbox != null && playerAlive != false)
		{
			_hitbox.ApplyAttack(_attack);
		}
	}
}