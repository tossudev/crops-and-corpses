using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Export] private float _maxHealth = 100.0f;
	private float _health;

	public override void _Ready()
	{
		_health = _maxHealth;
	}

	public void TakeDamage(Attack attack)
	{
		_health -= attack.damage;

		GD.Print(GetParent().Name + " health: " + _health);

		if (_health <= 0)
		{
			GetParent().QueueFree();
		}
	}
}
