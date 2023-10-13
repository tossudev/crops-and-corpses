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

	[ExportCategory("Settings")]
	[Export] private float maxZoom = 2f;
	[Export] private float minZoom = 1f;
	[Export] private int _speed = 100;

	private bool _canMelee = true;
	private bool _isDead = false;
	private Vector2 _knockback = Vector2.Zero;

	// to disable the player input, use:
	// PlayerController.SetProcessUnhandledInput(true/false);
    public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("left_click"))
		{
			_handheld.Use(_canMelee);
			_canMelee = false;
		}
		else if (@event.IsActionReleased("left_click"))
		{
			_handheld.Release();
			_canMelee = true;
		}

		else if (@event.IsActionPressed("wheel_up"))
		{
			if (_camera.Zoom.X < maxZoom)
				CameraZoom(0.1f);
		}
		else if (@event.IsActionPressed("wheel_down"))
		{
			if (_camera.Zoom.X > minZoom)
				CameraZoom(-0.1f);
		}

		else if(@event.IsActionPressed("pickup"))
		{
			_pickupArea.GetChild<CollisionShape2D>(0).Disabled = false;
		}
		else if(@event.IsActionReleased("pickup"))
		{
			_pickupArea.GetChild<CollisionShape2D>(0).Disabled = true;
		}
	}

    public override void _PhysicsProcess(double delta)
	{
		Movement();
	}

	private void CameraZoom(float zoomDelta)
	{
		Vector2 newZoom = _camera.Zoom += new Vector2(zoomDelta, zoomDelta);
		_camera.Zoom = newZoom;

		GD.Print(_camera.Zoom);
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
		var movement = GetMovementInputVector() * _speed;
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

		_sprite.Modulate = new Color(1, 0, 0, 1);
		knockbackTween.Parallel().TweenProperty(_sprite, "modulate", new Color(1, 1, 1, 1), duration);
	}
}
