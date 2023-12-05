using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[ExportCategory("Components")]
	[Export] private HandheldController _handheld;
	[Export] private Camera2D _camera;
	[Export] private Sprite2D _sprite;
	[Export] private Area2D _pickupArea;
	[Export] private PlayerSpriteController _rig;
	[Export] private HealthComponent _healthComponent;
	[Export] private StaminaComponent _staminaComponent;

	[Export] private Node2D _respawnPoint;

	[ExportCategory("Settings")]
	[Export] private float maxZoom = 2f;
	[Export] private float minZoom = 1f;
	[Export] private int _speed = 250;
	[Export] private float _runMultiplier = 1.5f;

	private bool _canMelee = true;
	private bool _isDead = false;
	private Vector2 _knockback = Vector2.Zero;
	private float speedMultiplier = 1;

	public bool canRun;
	public bool stopMovement;
	public float speedPercent = 1;
	static AudioController _audioController;


	public override void _Ready()
	{
		if (_isDead)
		{
			Position = _respawnPoint.Position;
			_isDead = false;
		}

		canRun = true;
		stopMovement = false;

		_audioController = GetNode<AudioController>("/root/Audio");
	}

	// to disable the player input, use:
	// player.SetProcessUnhandledInput(true/false);
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("left_click"))
		{
			_handheld.Use();
		}
		else if (@event.IsActionReleased("left_click"))
		{
			_handheld.Release();
		}

		if (@event.IsActionPressed("wheel_up"))
		{
			if (_camera.Zoom.X < maxZoom)
				CameraZoom(0.1f);
		}
		else if (@event.IsActionPressed("wheel_down"))
		{
			if (_camera.Zoom.X > minZoom)
				CameraZoom(-0.1f);
		}

		if (@event.IsActionPressed("pickup_item"))
		{
			_pickupArea.GetChild<CollisionShape2D>(0).Disabled = false;
		}
		else if (@event.IsActionReleased("pickup_item"))
		{
			_pickupArea.GetChild<CollisionShape2D>(0).Disabled = true;
		}

		if (@event.IsActionPressed("drop_item"))
		{
			if (PlayerInventoryController.isItemSelected)
			{
				PlayerInventoryController.DropSelectedItem(GetGlobalMousePosition(), FindParent("Objects"));
			}
		}

		for (int hotbarKey = 1; hotbarKey < 9; hotbarKey++)
		{
			if (@event.IsActionPressed("hotbar_" + hotbarKey.ToString()))
			{
				_handheld.Init();
			}
		}
	}

	// to disable the player input, use:
	// player.SetPhysicsProcess(true/false);
	public override void _PhysicsProcess(double delta)
	{
		Movement();
	}

	private void CameraZoom(float zoomDelta)
	{
		Vector2 newZoom = _camera.Zoom += new Vector2(zoomDelta, zoomDelta);
		_camera.Zoom = newZoom;
	}

	private Vector2 GetMovementInputVector()
	{
		Vector2 inputVector = Vector2.Zero;

		inputVector.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		inputVector.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		inputVector = inputVector.Normalized();

		return inputVector;
	}

	private void Movement()
	{
		if (Input.IsActionPressed("run") && !_handheld.isDrawing)
		{
			if (Velocity != Vector2.Zero && _staminaComponent.currentStamina > 0 && canRun)
			{
				speedMultiplier = _runMultiplier;
				_staminaComponent.drainRate = 0.3f;
				_staminaComponent.canDrain = true;
			}
			else
			{
				speedMultiplier = 1;
				_staminaComponent.canDrain = false;
				canRun = false;
			}
		}
		else if (Input.IsActionJustReleased("run") && !_handheld.isDrawing)
		{
			speedMultiplier = 1;
			_staminaComponent.canDrain = false;
			canRun = true;
		}

		var movement = GetMovementInputVector() * _speed * speedMultiplier * speedPercent;

		if (stopMovement)
		{
			movement = Vector2.Zero;
		}

		if (movement != Vector2.Zero)
		{
			if (speedMultiplier == _runMultiplier)
			{
				_audioController.PlayWalking(true);
			}
			else
			{
				_audioController.PlayWalking(false);
			}
		}

		Velocity = movement + _knockback;

		_rig.UpdateSprite(movement);

		MoveAndSlide();
	}

	private void AttackReceived(Attack attack)
	{
		var duration = 0.25f;
		_knockback = attack.direction * attack.knockback;

		var knockbackTween = GetTree().CreateTween();
		knockbackTween.Parallel().TweenProperty(this, "_knockback", new Vector2(0, 0), duration);

		_rig.Modulate = new Color(1, 0, 0, 1);
		knockbackTween.Parallel().TweenProperty(_rig, "modulate", new Color(1, 1, 1, 1), duration);
	}

	private void OnHealth(float health)
	{
		if (health <= 0)
		{
			_isDead = true;
			Respawn();
		}
	}

	private void Respawn()
	{
		PlayerInfo.sceneID = SceneID.Town;
		_healthComponent.SetHealth(_healthComponent.GetMaxHealth());
		_staminaComponent.SetStamina(_staminaComponent.GetMaxStamina());

		SceneManager.ChangeScene(this, Scene.Town);
	}

	void OnPickupAreaEntered(Area2D body)
	{
		var parent = body.GetParent();

		if (parent == null) return;

		DroppedItem itemScript = parent as DroppedItem;

		itemScript?.Pickup();
	}

	public void SaveState()
	{
		PlayerInfo.health = _healthComponent.GetHealth();
		PlayerInfo.stamina = _staminaComponent.GetStamina();
	}
}
