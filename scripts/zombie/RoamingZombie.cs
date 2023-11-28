using Godot;
using System;

public partial class RoamingZombie : CharacterBody2D
{
	[Export] private AudioStreamPlayer2D _audioStreamPlayer2D;
	[Export] private LootController _lootController;
	private enum ZombieOccupation{Miner,Farmer,Soldier,Woodcutter,Builder};
	private ZombieOccupation zombieOccupation;
	private Skeleton2D _sprite;
	private CharacterBody2D _player;
	private HitboxComponent[] _hitboxes;
	private Attack _attack;
	private Vector2 _knockback = Vector2.Zero;
	private Timer _timer;
	private Timer _updateStatsTimer;
	private ProgressBar _healthBar;
	private HealthComponent _healthComponent;
	private NodePath _rootNodePath;
	private Node2D rootNode;
	PackedScene instantiatedNPC;
	
	//private CompressedTexture2D strongZombieSprite;
	//private CompressedTexture2D mediumZombieSprite;
	private bool _playerInRange = false;
	private bool _fenceInRange = false;
	private ulong _entered;
	private ulong _exited;
	private bool _inTown;

	AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("Skeleton2D/AnimationPlayer");
		_hitboxes = new HitboxComponent[2];
		instantiatedNPC = (PackedScene)GD.Load("res://scenes/villager/villager.tscn");
		_rootNodePath = GetParent<Node2D>().GetPath();
		rootNode = GetNodeOrNull<Node2D>(_rootNodePath);
		_sprite = GetNodeOrNull<Skeleton2D>("Skeleton2D");
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

		int randomIndex = (int)GD.RandRange(1, 5); 

		
		if (randomIndex >= 1 && randomIndex <= 4)
		{
			var zombieHeadBonetNode = GetNode<Bone2D>("Skeleton2D/TorsoBone/HeadBone/"); //Skeleton2D/TorsoBone/HeadBone/ZombieHat1
			var zombieHatNode  = zombieHeadBonetNode.GetNode<Sprite2D>("ZombieHat"+randomIndex);
			zombieHatNode.Visible = true;
		}
		else
		{
			return;
		}
		switch(randomIndex)
		{
			case 1:
			zombieOccupation=ZombieOccupation.Farmer;
			break;
			case 2:
			zombieOccupation=ZombieOccupation.Soldier;
			break;
			case 3:
			zombieOccupation=ZombieOccupation.Miner;
			break;
			case 4:
			zombieOccupation=ZombieOccupation.Woodcutter;
			break;
			case 5:
			zombieOccupation=ZombieOccupation.Builder;
			break;
			default:
			break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		animationPlayer.Play("zombieWalk");
		Velocity += _knockback;
		MoveAndSlide();

		if (_sprite != null)
		{
			if (Mathf.Abs(Velocity.X) >= Mathf.Abs(Velocity.Y))
			{
				if (Velocity.X > 0.1f)
				{
					// Flip the character to face right
					_sprite.Scale = new Vector2(0.382f, 0.382f);
				}
				else
				{					
					// Flip the character to face left
					_sprite.Scale = new Vector2(-0.382f, 0.382f);
				}
			}
			else {
				if (Velocity.Y > 0.1f)
				{
					// Flip the character to face right
					_sprite.Scale = new Vector2(0.382f, 0.382f);
				}
				else
				{					
					// Flip the character to face left
					_sprite.Scale = new Vector2(-0.382f, 0.382f);
				}
			}
			
			if (Velocity.X == 0.1f)
			{
				animationPlayer.Play("zombieIdle");
			}
		}
	}
	public bool IsInTown()
	{
		return _inTown;
	}

	public void SetInTown()
	{
		_inTown = true;
	}

	private void AttackReceived(Attack attack)
	{
		var duration = 0.25f;
		_knockback = attack.direction * attack.knockback;

		var knockbackTween = GetTree().CreateTween();
		knockbackTween.Parallel().TweenProperty(this, "_knockback", new Vector2(0, 0), duration);

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

			ExpGain expGained = ZombieManager.type switch
			{
				ZombieManager.ZombieType.Weak => ExpGain.Small,
				ZombieManager.ZombieType.Medium => ExpGain.Small,
				ZombieManager.ZombieType.Strong => ExpGain.Medium,
				_ => throw new ArgumentOutOfRangeException()
			};

			TownManager.GainExp(expGained);

			QueueFree();
		}

		_lootController.CallDeferred("OnHealth", _health);
	}

	private void OnAttackBoxEntered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			_player = (CharacterBody2D)body;
			_playerInRange = true;

			// direction from zombie to player
			Vector2 _direction = (_player.GlobalPosition - this.GlobalPosition).Normalized();
			_attack.direction = _direction;

			_hitboxes[0] = _player.GetNodeOrNull<HitboxComponent>("HitboxComponent");

			if (_hitboxes[0] != null)
			{
				//_hitbox.ApplyAttack(_attack);
				if (_timer.TimeLeft <= 0)
				{
					_timer.Start();
				}
			}
			else
			{
				GD.Print("ZOMBIE: No hitbox found on player");
			}
		}

		if (body.IsInGroup("fence") || body.IsInGroup("building"))
		{
			_fenceInRange = true;
			_entered = body.GetInstanceId();

			// direction from zombie to fence/building
			Vector2 _direction = (body.GlobalPosition - this.GlobalPosition).Normalized();
			_attack.direction = _direction;

			_hitboxes[1] = body.GetParent().GetNodeOrNull<HitboxComponent>("HitboxComponent");

			if (_hitboxes[1] != null)
			{
				if (_timer.TimeLeft <= 0)
				{
					_timer.Start();
				}
			}
			else
			{
				// GD.Print("ZOMBIE: No hitbox found on fence/building. ");
			}
		}
	}

	private void OnAttackBoxExited(Node2D body)
	{
		_exited = body.GetInstanceId();
		if (body.IsInGroup("player"))
		{
			_playerInRange = false;
		}

		if (body.IsInGroup("fence") || body.IsInGroup("building"))
		{
			if (_entered == _exited)
			{
				_fenceInRange = false;
			}
		}

		if (!_playerInRange && !_fenceInRange)
		{
			_timer.Stop();
		}
	}
	private void OnTimerTimeout()
	{
		if (_playerInRange && _hitboxes[0] != null && ZombieManager.playerAlive)
		{
			_hitboxes[0].ApplyAttack(_attack);
		}
		else if (_fenceInRange && _hitboxes[1] != null)
		{
			_hitboxes[1].ApplyAttack(_attack);
		}
	}

	private void OnUpdateStatsTimeout()
	{
		{
			switch (ZombieManager.type)
			{
				case ZombieManager.ZombieType.Weak:
					break;
				case ZombieManager.ZombieType.Medium:
					break;
				case ZombieManager.ZombieType.Strong:
				default:
					break;
			}

			/* if (ZombieManager.type == ZombieManager.ZombieType.Weak)
			{
			}
			else if (ZombieManager.type == ZombieManager.ZombieType.Medium)
			{
			}
			else if (ZombieManager.type == ZombieManager.ZombieType.Strong)
			{
			} */
			_attack.damage = ZombieManager.damage;
			_timer.WaitTime = ZombieManager.attackTime;
		}
	}
}