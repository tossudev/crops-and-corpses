using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[Export]
	private int _speed = 100;

	private bool _canMelee = false;
	private float _health = 100f;
	private bool _isDead = false;

	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		Movement();
	}

	private Vector2 GetInputVector()
	{
		Vector2 inputVector = Vector2.Zero;

		inputVector.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		inputVector.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		inputVector = inputVector.Normalized();

		return inputVector;
	}

	private void Movement()
	{
		Vector2 inputVector = GetInputVector();
		Velocity = inputVector * _speed;

		MoveAndSlide();
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
