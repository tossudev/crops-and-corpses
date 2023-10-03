using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[Export] private WeaponController _weapon;
	[Export] private int _speed = 100;

	private bool _canMelee = true;
	private bool _isDead = false;

	public override void _PhysicsProcess(double delta)
	{
		Movement();

		if (Input.IsActionPressed("left_click"))
		{
			_weapon.Use(_canMelee);
			_canMelee = false;
		}
		else if (Input.IsActionJustReleased("left_click"))
		{
			_weapon.ReleaseDraw();
			_canMelee = true;
		}
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
