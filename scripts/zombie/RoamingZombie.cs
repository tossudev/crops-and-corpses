using Godot;
using System;
using System.Diagnostics.Tracing;

public partial class RoamingZombie : CharacterBody2D
{
	// [Export] private float _damage;
	[Export] private AudioStreamPlayer2D _audioStreamPlayer2D;
	private Sprite2D _sprite;
	private CharacterBody2D _player;
	private Node2D _fence;
	private HitboxComponent _hitbox;
	private Attack _attack;
	private Vector2 _knockback = Vector2.Zero;
	private Timer _timer;
	private Timer _updateStatsTimer;
	private ProgressBar _healthBar;
	private HealthComponent _healthComponent;
	private NodePath _rootNodePath;
	private Node2D rootNode;
	PackedScene instantiatedNPC;
	private CompressedTexture2D strongZombieSprite;
	private CompressedTexture2D mediumZombieSprite;

	public override void _Ready()
	{
		instantiatedNPC = (PackedScene)GD.Load("res://scenes/villager/villager.tscn");
		_rootNodePath = GetParent<Node2D>().GetPath();
		rootNode = GetNodeOrNull<Node2D>(_rootNodePath);
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		_timer = GetNodeOrNull<Timer>("AttackTimer");
		_updateStatsTimer = GetNodeOrNull<Timer>("UpdateStatsTimer");
		_healthBar = GetNodeOrNull<ProgressBar>("HealthBar");
		_healthComponent = GetNodeOrNull<HealthComponent>("HealthComponent");
		_audioStreamPlayer2D = GetNodeOrNull<AudioStreamPlayer2D>("ZombieNoise");

		_attack = new Attack
		{
			damage = ZombieManager.damage,
			knockback = 500f
		};

		_updateStatsTimer.Start();		
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

	public void ChangeZombieNoise(AudioStream audioStream)
	{
		if(audioStream != null)
		{
			_audioStreamPlayer2D.Stream = audioStream;
			_audioStreamPlayer2D.Play();
		}		
	}

	private void AttackReceived(Attack attack)
	{
		// var duration = 0.25f;
		// _knockback = attack.direction * attack.knockback;

		// var knockbackTween = GetTree().CreateTween();
		// knockbackTween.Parallel().TweenProperty(this, "_knockback", new Vector2(0, 0), duration);
		
		/* GD.Print("2");
		GD.Print(attack.effect); */
		switch (attack.effect)
		{
			case EffectType.Cure:

				SpawnScript.RemoveZombieFromList(this);
				Transform2D zombiePos = this.Transform;
				CharacterBody2D spawnNPC = (CharacterBody2D)instantiatedNPC.Instantiate();
				spawnNPC.Transform = zombiePos;
				rootNode.AddChild(spawnNPC);
				spawnNPC.Scale = new Vector2(0.5f, 0.5f);
				QueueFree();
				break;
			default:
				break;
		}
	}
	private void OnHealth(float _health)
	{
		if (_health <= 0)
		{
			SpawnScript.RemoveZombieFromList(this);
			GD.Print("Check");
			QueueFree();
		}
	}

	private void OnAttackBoxEntered(Node2D body)
	{
		// GD.Print("Collision with: " + body.Name);
		if (body.IsInGroup("player"))
		{
			_player = (CharacterBody2D)body;

			// direction from zombie to player
			Vector2 _direction = (_player.GlobalPosition - this.GlobalPosition).Normalized();
			_attack.direction = _direction;

			_hitbox = _player.GetNodeOrNull<HitboxComponent>("HitboxComponent");

			if (_hitbox != null)
			{
				//_hitbox.ApplyAttack(_attack);
				_timer.Start();
			}
			else
			{
				GD.Print("ZOMBIE: No hitbox found on player");
			}
		}

		if(body.IsInGroup("fence"))
		{			
			_fence = (Node2D)body;

			// direction from zombie to fence
			Vector2 _direction = (_fence.GlobalPosition - this.GlobalPosition).Normalized();
			_attack.direction = _direction;

			_hitbox = _fence.GetParent().GetNodeOrNull<HitboxComponent>("HitboxComponent");

			if(_hitbox != null)
			{
				_timer.Start();
			}
			else
			{
				GD.Print("ZOMBIE: No hitbox found on fence");
			}
		}
	}

	private void UpdateHealth()
	{
		// update health bar when player damages zombie
		if (_healthComponent != null)
		{
			_healthBar.Value = _healthComponent.health;

			if (_healthComponent.health >= 100)
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
		if (_hitbox != null && ZombieManager.playerAlive != false)
		{
			_hitbox.ApplyAttack(_attack);
		}
	}

	private void OnUpdateStatsTimeout()
	{		
		{
			if(ZombieManager.type == ZombieManager.ZombieType.Weak)
			{
				mediumZombieSprite = GD.Load<CompressedTexture2D>("res://LilianTests/Sprites/zombie_placeholder.png");
				_sprite.Texture = mediumZombieSprite;
			}
			else if(ZombieManager.type == ZombieManager.ZombieType.Medium)
			{
				mediumZombieSprite = GD.Load<CompressedTexture2D>("res://LilianTests/Sprites/zombie_placeholder.png");
				_sprite.Texture = mediumZombieSprite;
				
			}
			else if(ZombieManager.type == ZombieManager.ZombieType.Strong)
			{
				GD.Print("Strong");
				strongZombieSprite = GD.Load<CompressedTexture2D>("res://DaniTests/Sprites/strongZombie.png");
				_sprite.Texture = strongZombieSprite;
			}
			_attack.damage = ZombieManager.damage;
			_timer.WaitTime = ZombieManager.attackTime;
			//GD.Print("DMG: " + _attack.damage + "\nwait time: " + _timer.WaitTime);
		}		
	}	
}