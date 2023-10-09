using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[Export] private HandheldController _handheld;
	[Export] private Camera2D _camera;
	[Export] private float maxZoom = 2f;
	[Export] private float minZoom = 1f;
	[Export] private int _speed = 100;

	private bool _canMelee = true;
	private bool _isDead = false;

	public override void _PhysicsProcess(double delta)
	{
		Movement();

		if (Input.IsActionPressed("left_click"))
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
		Vector2 movementVector = GetMovementInputVector();
		Velocity = movementVector * _speed;

		MoveAndSlide();
	}
}
