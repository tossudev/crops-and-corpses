using Godot;
using System;

public partial class player_controller : CharacterBody2D
{
	[Export]
	private int _speed = 100;

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
}
