using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[ExportCategory("Components")]
	[Export] private HandheldController _handheld;
	[Export] private Camera2D _camera;
	[Export] private Sprite2D _sprite;
	[Export] private Area2D _pickupArea;

	[ExportCategory("Settings")]
	[Export] private float maxZoom = 2f;
	[Export] private float minZoom = 1f;
	[Export] private int _speed = 100;

	private bool _canMelee = true;
	private bool _isDead = false;
	private Vector2 _knockback = Vector2.Zero;

	public override void _PhysicsProcess(double delta)
	{
		Movement();

		if (Input.IsActionJustPressed("left_click"))
		{
			_handheld.Use(_canMelee);
			_canMelee = false;
		}
		else if (Input.IsActionJustReleased("left_click"))
		{
			_handheld.Release();
			_canMelee = true;
		}

		else if (Input.IsActionJustPressed("wheel_up"))
		{
			if (_camera.Zoom.X < maxZoom)
				CameraZoom(0.1f);
		}
		else if (Input.IsActionJustPressed("wheel_down"))
		{
			if (_camera.Zoom.X > minZoom)
				CameraZoom(-0.1f);
		}

		else if(Input.IsActionJustPressed("pickup"))
		{
			_pickupArea.GetChild<CollisionShape2D>(0).Disabled = false;
		}
		else if(Input.IsActionJustReleased("pickup"))
		{
			_pickupArea.GetChild<CollisionShape2D>(0).Disabled = true;
		}
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
