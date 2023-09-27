using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[Export]
	private int _speed = 100;

	private bool _canMelee = false;
	private float _health = 100f;
	private bool _isDead = false;

	Node2D _weapons;

	[Export(PropertyHint.Range, "0,5,")]
	private float _swingCD; //temp

	private Timer _attackCooldownTimer;

	public override void _Ready()
	{
		_weapons = GetNode<Node2D>("Weapons");
		_attackCooldownTimer = _weapons.GetNode<Timer>("SwingCooldown");
	}

	public override void _PhysicsProcess(double delta)
	{
		Movement();

		if (_attackCooldownTimer.IsStopped() == true)
		{
			_weapons.GetNode<Node2D>("Sword/Sprite2D").Modulate = new Color(1, 0, 0, 0.5f);
		}

		if (Input.IsActionJustPressed("left_click") && _attackCooldownTimer.IsStopped())
		{
			MeleeStrike();
		}
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

	private Vector2 GetCursorVector()
	{
		Vector2 cursorPosition = GetGlobalMousePosition();
		Vector2 playerPosition = GlobalPosition;
		Vector2 cursorVector = cursorPosition - playerPosition;
		cursorVector = cursorVector.Normalized();

		return cursorVector;
	}

	private void MeleeStrike()
	{
		if (_canMelee != true)
			return;

		_attackCooldownTimer.Start(_swingCD);

		Vector2 cursorVector = GetCursorVector();
		float angle = cursorVector.Angle() * 180 / Mathf.Pi;
		angle = Mathf.Round(angle / 45) * 45;
		angle -= 90;

		GD.Print(angle);

		_weapons.GetNode<Node2D>("Sword/Sprite2D").Modulate = new Color(1, 0, 0, 0.25f);
		_weapons.GetNode<Node2D>("Sword").RotationDegrees = angle;
	}
}
