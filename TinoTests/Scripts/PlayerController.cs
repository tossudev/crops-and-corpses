using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[Export] private WeaponController _weapon;
	[Export] private int _speed = 100;

	private bool _canMelee = false;
	private bool _isDead = false;

	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		Movement();

		if (Input.IsActionJustPressed("left_click") || Input.IsActionJustReleased("left_click"))
		{
			UseWeapon();
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

	private Vector2 GetCursorVector()
	{
		Vector2 cursorPosition = GetGlobalMousePosition();
		Vector2 playerPosition = GlobalPosition;
		Vector2 cursorVector = cursorPosition - playerPosition;
		cursorVector = cursorVector.Normalized();

		return cursorVector;
	}

	// Vector2 cursorVector = GetCursorVector();
	// float angle = cursorVector.Angle() * 180 / Mathf.Pi;
	// angle = Mathf.Round(angle / 45) * 45;
	// angle -= 90;

	private void UseWeapon()
	{
		// Vector2 cursorVector = GetCursorVector();
		// float angle = cursorVector.Angle() * 180 / Mathf.Pi;
		// angle -= 90;


		_weapon.Use(GetCursorVector());
	}



	int enemyLayer = 3;
	int interactableLayer = 4;
	private void OnHitboxEntered(Area2D body)
	{
		if (body.GetCollisionLayerValue(enemyLayer) == true)
		{
			_canMelee = true;
			GD.Print("Player: ouuch!");
		}
		else if (body.GetCollisionLayerValue(interactableLayer) == true)
		{
			GD.Print("Player: I'm interacting!");
		}
	}

	private void OnHitboxExited(Area2D body)
	{
		if (body.GetCollisionLayerValue(enemyLayer) == true)
		{
			_canMelee = false;
			GD.Print("Player: Whew");
		}
		else if (body.GetCollisionLayerValue(interactableLayer) == true)
		{
			GD.Print("Player: I'm not interacting anymore!");
		}
	}
}
